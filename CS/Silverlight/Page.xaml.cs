using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DevExpress.AgDataGrid;
using DevExpress.AgDataGrid.Internal;

namespace DragAndDrop {
	public partial class Page : UserControl, IDropableObject {		
		public class DataItem {
			public string Value {
				get;
				set;
			}
		}

		DropController leftController;
		DropController rightController;

		public Page() {
			InitializeComponent();
			
			//
			// Create DragAndDrop Controllers Before Populating the grid
			//

			leftController = new DropController(this.leftGrid, this, this.DragSurface);
			rightController = new DropController(this.rightGrid, this, this.DragSurface);
				

			//
			// Populate Grids with Data
			//

			this.leftGrid.DataSource = new List<DataItem>() {
				new DataItem() { Value = "Item 0" },
				new DataItem() { Value = "Item 1" },
				new DataItem() { Value = "Item 2" },
				new DataItem() { Value = "Item 3" },
				new DataItem() { Value = "Item 4" },
			};
			this.rightGrid.DataSource = new List<DataItem>() {
				new DataItem() { Value = "Item 5" },
				new DataItem() { Value = "Item 6" },
				new DataItem() { Value = "Item 7" },
				new DataItem() { Value = "Item 8" },
				new DataItem() { Value = "Item 9" },
			};
		}		

		#region IDropableObject Members

		void IDropableObject.AcceptDrag(IDragableObject dragObject, Point position) {
			DragObject obj = dragObject as DragObject;
			if(obj == null) {
				return;
			}

			// 
			// Recycle
			//
			if(this.leftGrid == obj.Source || this.rightGrid == obj.Source) {
				Point localPt = DropController.TransformSurface(position, this.recycler, this.DragSurface);
				if(DropController.IsIn(this.recycler, localPt)) {
					IList<DataItem> items = obj.Source.DataSource as IList<DataItem>;
					items.Remove(obj.DataRow as DataItem);
					obj.Source.Refresh();
					return;
				}
			}

			IList<DataItem> sourceItems = null;
			IList<DataItem> destItems = null;

			// 
			// Right to Left
			//
			if(obj.Source == this.rightGrid) {
				sourceItems = this.rightGrid.DataSource as IList<DataItem>;
				destItems = this.leftGrid.DataSource as IList<DataItem>;
			}

			// 
			// Left to Right
			//
			else if(obj.Source == this.leftGrid) {
				sourceItems = this.leftGrid.DataSource as IList<DataItem>;
				destItems = this.rightGrid.DataSource as IList<DataItem>;
			}

			DataItem dataItem = obj.DataRow as DataItem;
			if(sourceItems != null && destItems != null && dataItem != null && sourceItems.Contains(dataItem) && !destItems.Contains(dataItem)) {
				sourceItems.Remove(dataItem);
				destItems.Add(dataItem);

				this.leftGrid.Refresh();
				this.rightGrid.Refresh();
			}
		}

		bool IDropableObject.CanAccept(IDragableObject dragObject, Point position) {
			DragObject obj = dragObject as DragObject;
			if(obj == null) return false;

			// 
			// Recycle
			//
			if(this.leftGrid == obj.Source || this.rightGrid == obj.Source) {
				
				Point localPt = DropController.TransformSurface(position, this.recycler, this.DragSurface);
				if(DropController.IsIn(this.recycler, localPt)) {
					return true;
				}


			}

			// 
			// Left to Right
			//
			if(this.leftGrid == obj.Source) {
				Point localPt = DropController.TransformSurface(position, this.rightGrid, this.DragSurface);
				if(DropController.IsIn(this.rightGrid, localPt)) {
					return true;
				}
			}
			
			// 
			// Right to Left
			//
			else if(this.rightGrid == obj.Source) {
				Point localPt = DropController.TransformSurface(position, this.leftGrid, this.DragSurface);
				if(DropController.IsIn(this.leftGrid, localPt)) {
					return true;
				}
			}

			return false;
		}

		bool IDropableObject.CanAccept() {
			return true;
		}

		FrameworkElement IDropableObject.GetThumbObject() {
			return null;
		}

		Rect IDropableObject.GetThumbRect(Point position) {
			return new Rect();
		}

		#endregion
	}
}