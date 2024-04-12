using Sunny.UI;
using System;
using System.Drawing;
using System.Windows.Forms;

public class DragSortableListBox : ListBox
{
    private bool isDragging = false;
    private int dragStartIndex = -1;
    private Point dragStartPoint;
    private Point dragCurrentPoint;

    public DragSortableListBox()
    {
        this.DoubleBuffered = true; // 减少拖动时的闪烁  
        this.MouseDown += new MouseEventHandler(DragSortableListBox_MouseDown);
        this.MouseMove += new MouseEventHandler(DragSortableListBox_MouseMove);
        this.MouseUp += new MouseEventHandler(DragSortableListBox_MouseUp);
    }

    private void DragSortableListBox_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            int index = IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                dragStartIndex = index;
                dragStartPoint = e.Location;
                isDragging = true;
            }
        }
    }

    private void DragSortableListBox_MouseMove(object sender, MouseEventArgs e)
    {
        if (isDragging)
        {
            dragCurrentPoint = e.Location;

            // 检查是否应该开始拖动排序  
            if (Math.Abs(dragCurrentPoint.Y - dragStartPoint.Y) > SystemInformation.DoubleClickSize.Height)
            {
                // 这里可以开始实际的拖动排序逻辑  
                // ...  
            }
        }
    }

    private void DragSortableListBox_MouseUp(object sender, MouseEventArgs e)
    {
        if (isDragging)
        {
            isDragging = false;

            // 检查鼠标释放的位置，并更新项的顺序  
            int endIndex = IndexFromPoint(e.Location);
            if (endIndex != ListBox.NoMatches && endIndex != dragStartIndex)
            {
                object item = Items[dragStartIndex];
                Items.RemoveAt(dragStartIndex);
                Items.Insert(endIndex, item);
                SelectedIndex = endIndex; // 可选：设置选中项为移动后的项  
            }
        }
    }
}