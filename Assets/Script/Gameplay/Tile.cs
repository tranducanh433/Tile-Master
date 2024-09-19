using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum TILE_STATE
{
    LOCKED,
    CLICKABLE,
    SELECTED,
}

public class Tile : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] Color m_blockedStateColor;
    [SerializeField] float m_checkTileRadius = 0.5f;
    [SerializeField] LayerMask tileLayerMask;

    [Header("Component")]
    [SerializeField] SpriteRenderer m_sr;
    [SerializeField] Collider2D m_collider;

    [Header("Event")]
    public UnityEvent<Tile> OnClick = new UnityEvent<Tile>();

    private TILE_STATE m_currentState = TILE_STATE.CLICKABLE;

    public SpriteRenderer sr { get { return m_sr; } }
    public Sprite sprite { get { return m_sr.sprite; } }


    private void Start()
    {
        UpdateState();
    }

    public void UpdateState()
    {
        Collider2D[] _tiles = Physics2D.OverlapCircleAll(transform.position, m_checkTileRadius, tileLayerMask);
        if(_tiles.Length <= 0)
            return;

        foreach(Collider2D _tile in _tiles)
        {
            SpriteRenderer _tileSR = _tile.GetComponent<SpriteRenderer>();
            if(_tileSR.sortingOrder > sr.sortingOrder)
            {
                SetToLockedState();
                return;
            }
        }

        m_currentState = TILE_STATE.CLICKABLE;
        sr.color = Color.white;
    }

    public void SetToLockedState()
    {
        m_currentState = TILE_STATE.LOCKED;
        sr.color = m_blockedStateColor;
    }
    public void SetToClickableState()
    {
        m_currentState = TILE_STATE.CLICKABLE;
        sr.color = Color.white;
    }
    public void SetToSelectedState()
    {
        m_currentState = TILE_STATE.SELECTED;
        sr.color = Color.white;
        m_collider.enabled = false;
        UpdateStateAllTileBehind();
    }


    private void UpdateStateAllTileBehind()
    {
        Collider2D[] _tiles = Physics2D.OverlapCircleAll(transform.position, m_checkTileRadius, tileLayerMask);
        if (_tiles.Length <= 0)
            return;

        foreach (Collider2D _tile in _tiles)
        {
            _tile.GetComponent<Tile>().UpdateState();
        }
    }



    private void OnMouseDown()
    {
        if(m_currentState == TILE_STATE.CLICKABLE)
        {
            OnClick.Invoke(this);
        }
    }
}
