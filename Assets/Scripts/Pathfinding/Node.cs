public class Node
{
    public CellData cell;

    public int gCost;
    public int hCost;

    public int fCost => gCost + hCost;

    public Node parent;

    public Node(CellData cell, int gCost, int hCost, Node parent)
    {
        this.cell = cell;
        this.gCost = gCost;
        this.hCost = hCost;
        this.parent = parent;
    }
}
