using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridDirection
{
    public readonly Vector2Int Vector;

    private GridDirection(int x, int y)
    {
        Vector = new Vector2Int(x, y);
    }

    // GridDirection을 Vector2Int로 암시적 형변환 
    public static implicit operator Vector2Int(GridDirection direction) => direction.Vector;

    // ※ DefaultIfEmpty(None) : CardinalAndIntercardinalDirections 리스트가 비어 있을 경우, None을 기본 값을 반환 
    // ※ FirstOrDefault : 조건에 맞는 첫 번째 요소를 반환하거나, 조건에 맞는 요소가 없을 경우 기본값(null)을 반환
    // → 리스트의 각 direction이 입력된 vector와 같은지 비교
    public static GridDirection GetDirectionFromV2I(Vector2Int vector)
        => CardinalAndIntercardinalDirections.DefaultIfEmpty(None).FirstOrDefault(direction => direction == vector);

    public static readonly GridDirection None = new GridDirection(0, 0);
    public static readonly GridDirection North = new GridDirection(0, 1);
    public static readonly GridDirection South = new GridDirection(0, -1);
    public static readonly GridDirection East = new GridDirection(1, 0);
    public static readonly GridDirection West = new GridDirection(-1, 0);
    public static readonly GridDirection NorthEast = new GridDirection(1, 1);
    public static readonly GridDirection NorthWest = new GridDirection(-1, 1);
    public static readonly GridDirection SouthEast = new GridDirection(1, -1);
    public static readonly GridDirection SouthWest = new GridDirection(-1, -1);

    public static readonly List<GridDirection> CardinalDirections = new List<GridDirection>()
    {
        North,
        East,
        South,
        West
    };

    public static readonly List<Vector2Int> CardinalDirectionsByVector2Int = new List<Vector2Int>()
    {
        North,
        East,
        South,
        West
    };

    public static readonly List<GridDirection> CardinalAndIntercardinalDirections = new List<GridDirection>()
    {
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest
    };

    public static readonly List<GridDirection> AllDirections = new List<GridDirection>()
    {
        None,
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest
    };
}
