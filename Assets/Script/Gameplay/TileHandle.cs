using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TileHandle : MonoBehaviour
{
    [SerializeField] List<Tile> m_tilesInThisLevel = new List<Tile>();
    [SerializeField] Transform[] m_tilePositions;
    [SerializeField] LinkedList<Tile> m_selectedTiles = new LinkedList<Tile>();

    private void Start()
    {
        foreach (Tile tile in m_tilesInThisLevel)
        {
            tile.OnClick.AddListener(OnClickTile);
        }
    }

    public void CheckTileHandle()
    {
        StartCoroutine(CheckTileHandleCO());
    }

    public void OnClickTile(Tile clickedTile)
    {
        if (m_selectedTiles.Count >= m_tilePositions.Length)
            return;

        clickedTile.SetToSelectedState();

        LinkedListNode<Tile> foundTile = FindLast(clickedTile);
        LinkedListNode<Tile> clickedTile_node = foundTile == null
                                                    ? m_selectedTiles.AddLast(clickedTile)
                                                    : m_selectedTiles.AddAfter(foundTile, clickedTile);

        UpdateTilePosition(clickedTile_node);
    }

    private LinkedListNode<Tile> FindLast(Tile tile)
    {
        LinkedListNode<Tile> rs = null;

        LinkedListNode<Tile> currentTile = m_selectedTiles.First;
        while (currentTile != null)
        {
            if(currentTile.Value.sprite == tile.sprite)
            {
                rs = currentTile;
            }

            currentTile = currentTile.Next;
        }

        return rs;
    }

    private void UpdateTilePosition(LinkedListNode<Tile> tileToAddListener)
    {
        LinkedListNode<Tile> currentTile = m_selectedTiles.First;
        int currentIndex = 0;
        while(currentTile != null)
        {
            if(currentIndex < m_tilePositions.Length)
            {
                Follow tile_follow = currentTile.Value.AddComponent<Follow>();
                tile_follow.SetFollowTarget(m_tilePositions[currentIndex]);

                if(currentTile ==  tileToAddListener)
                    tile_follow.OnFinishFollow.AddListener(CheckTileHandle);
            }

            currentTile = currentTile.Next;
            currentIndex++;
        }
    }

    IEnumerator CheckTileHandleCO()
    {
        List<LinkedListNode<Tile>> match3Tiles = new List<LinkedListNode<Tile>>();
        LinkedListNode<Tile> currentTile = m_selectedTiles.First;
        while (currentTile != null)
        {
            // Check if current tile as same as previous tile
            if(match3Tiles.Count == 0)
            {
                match3Tiles.Add(currentTile);
            }
            else if (match3Tiles[0].Value.sprite == currentTile.Value.sprite)
            {
                match3Tiles.Add(currentTile);
            }
            else
            {
                match3Tiles.Clear();
                match3Tiles.Add(currentTile);
            }

            // if 3 same tile was next to each other => play animation and remove
            if (match3Tiles.Count >= 3)
            {
                foreach (LinkedListNode<Tile> tile in match3Tiles)
                {
                    tile.Value.GetComponent<Animator>().SetTrigger("Disapear");
                    m_selectedTiles.Remove(tile);
                    m_tilesInThisLevel.Remove(tile.Value);
                }
            }

            currentTile = currentTile.Next;
        }

        yield return new WaitForSeconds(0.35f);

        UpdateTilePosition(null);

        if (m_tilesInThisLevel.Count == 0)
            PuzzleSolve();
        if (m_selectedTiles.Count >= m_tilePositions.Length)
            FailedToSolve();
    }

    private void PuzzleSolve()
    {
        GameManager.Instance.DisplayWinText();
    }
    private void FailedToSolve()
    {
        GameManager.Instance.DisplayLoseText();
    }
}


[System.Serializable]
public class TileSlot
{
    [SerializeField] Transform m_tilePoint;
    [SerializeField] Tile m_tile;

    public Transform tilePoint { get { return m_tilePoint; } }
    public Tile tile { get { return m_tile; } set { m_tile = value; } }
}
