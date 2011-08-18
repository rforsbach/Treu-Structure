using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.ComponentModel;
using Canguro.Model;

namespace Canguro.Controller.Grid
{
    class SelectSimilarCmd
    {
        public static void SelectLines(GridView<LineElement> grid)
        {
            SelectItems<LineElement>(grid, Model.Model.Instance.LineList);
        }

        public static void SelectJoints(GridView<Joint> grid)
        {
            SelectItems<Joint>(grid, Model.Model.Instance.JointList);
        }

        public static void SelectAreas(GridView<AreaElement> grid)
        {
            SelectItems<AreaElement>(grid, Model.Model.Instance.AreaList);
        }

        private static void SelectItems<TItem>(GridView<TItem> grid, IList items) where TItem : Item
        {
            try
            {
                if (grid.CurrentCell != null)
                {
                    int column = grid.CurrentCell.ColumnIndex;
                    object value = grid.CurrentCell.Value;
                    List<PropertyDescriptor> propList = grid.Properties;
                    foreach (Item item in items)
                    {
                        if (item != null)
                        {
                            if (item is LineElement && value is Joint)
                            {
                                item.IsSelected = Equals(((LineElement)item).I, value) || Equals(((LineElement)item).J, value);
                            }
                            else
                            {
                                item.IsSelected = Equals(propList[column].GetValue(item), value);
                            }
                        }
                    }
                    Model.Model.Instance.ChangeSelection(null);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private static bool Equals(object objA, object objB)
        {
            if (objA is JointDOF)
                return objA.ToString().Equals(objB.ToString());
            else if (objA is Model.StraightFrameProps)
                return ((StraightFrameProps)objA).Section.Name.Equals(((StraightFrameProps)objB).Section.Name);
            else if (objA is Joint && objB is Joint)
                return (((Joint)objA).Id == ((Joint)objB).Id);
            else if (objA is float && objB is float)
                return Math.Round((Convert.ToDecimal((float)objA)), 4).Equals(Math.Round(Convert.ToDecimal((float)objB), 4));
            else if (objA is double && objB is double)
                return Math.Round((Convert.ToDecimal((float)objA)), 4).Equals(Math.Round(Convert.ToDecimal((float)objB), 4));
            else
                return object.Equals(objA, objB);
        }
    }
}
