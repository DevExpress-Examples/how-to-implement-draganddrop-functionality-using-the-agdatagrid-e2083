Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Net
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Shapes
Imports DevExpress.AgDataGrid
Imports DevExpress.AgDataGrid.Internal

Namespace DragAndDrop
	Partial Public Class Page
		Inherits UserControl
		Implements IDropableObject
		Public Class DataItem
			Private privateValue As String
			Public Property Value() As String
				Get
					Return privateValue
				End Get
				Set(ByVal value As String)
					privateValue = value
				End Set
			End Property
		End Class

		Private leftController As DropController
		Private rightController As DropController

		Public Sub New()
			InitializeComponent()

			'
			' Create DragAndDrop Controllers Before Populating the grid
			'

			leftController = New DropController(Me.leftGrid, Me, Me.DragSurface)
			rightController = New DropController(Me.rightGrid, Me, Me.DragSurface)


			'
			' Populate Grids with Data
			'

			Me.leftGrid.DataSource = New List(Of DataItem) (New DataItem() {New DataItem() With {.Value = "Item 0"}, New DataItem() With {.Value = "Item 1"}, New DataItem() With {.Value = "Item 2"}, New DataItem() With {.Value = "Item 3"}, New DataItem() With {.Value = "Item 4"}})
			Me.rightGrid.DataSource = New List(Of DataItem) (New DataItem() {New DataItem() With {.Value = "Item 5"}, New DataItem() With {.Value = "Item 6"}, New DataItem() With {.Value = "Item 7"}, New DataItem() With {.Value = "Item 8"}, New DataItem() With {.Value = "Item 9"}})
		End Sub

		#Region "IDropableObject Members"

		Private Sub AcceptDrag(ByVal dragObject As IDragableObject, ByVal position As Point) Implements IDropableObject.AcceptDrag
			Dim obj As DragObject = TryCast(dragObject, DragObject)
			If obj Is Nothing Then
				Return
			End If

			' 
			' Recycle
			'
			If Me.leftGrid Is obj.Source OrElse Me.rightGrid Is obj.Source Then
				Dim localPt As Point = DropController.TransformSurface(position, Me.recycler, Me.DragSurface)
				If DropController.IsIn(Me.recycler, localPt) Then
					Dim items As IList(Of DataItem) = TryCast(obj.Source.DataSource, IList(Of DataItem))
					items.Remove(TryCast(obj.DataRow, DataItem))
					obj.Source.Refresh()
					Return
				End If
			End If

			Dim sourceItems As IList(Of DataItem) = Nothing
			Dim destItems As IList(Of DataItem) = Nothing

			' 
			' Right to Left
			'
			If obj.Source Is Me.rightGrid Then
				sourceItems = TryCast(Me.rightGrid.DataSource, IList(Of DataItem))
				destItems = TryCast(Me.leftGrid.DataSource, IList(Of DataItem))

			' 
			' Left to Right
			'
			ElseIf obj.Source Is Me.leftGrid Then
				sourceItems = TryCast(Me.leftGrid.DataSource, IList(Of DataItem))
				destItems = TryCast(Me.rightGrid.DataSource, IList(Of DataItem))
			End If

			Dim dataItem As DataItem = TryCast(obj.DataRow, DataItem)
			If sourceItems IsNot Nothing AndAlso destItems IsNot Nothing AndAlso dataItem IsNot Nothing AndAlso sourceItems.Contains(dataItem) AndAlso (Not destItems.Contains(dataItem)) Then
				sourceItems.Remove(dataItem)
				destItems.Add(dataItem)

				Me.leftGrid.Refresh()
				Me.rightGrid.Refresh()
			End If
		End Sub

		Private Function CanAccept(ByVal dragObject As IDragableObject, ByVal position As Point) As Boolean Implements IDropableObject.CanAccept
			Dim obj As DragObject = TryCast(dragObject, DragObject)
			If obj Is Nothing Then
				Return False
			End If

			' 
			' Recycle
			'
			If Me.leftGrid Is obj.Source OrElse Me.rightGrid Is obj.Source Then

				Dim localPt As Point = DropController.TransformSurface(position, Me.recycler, Me.DragSurface)
				If DropController.IsIn(Me.recycler, localPt) Then
					Return True
				End If


			End If

			' 
			' Left to Right
			'
			If Me.leftGrid Is obj.Source Then
				Dim localPt As Point = DropController.TransformSurface(position, Me.rightGrid, Me.DragSurface)
				If DropController.IsIn(Me.rightGrid, localPt) Then
					Return True
				End If

			' 
			' Right to Left
			'
			ElseIf Me.rightGrid Is obj.Source Then
				Dim localPt As Point = DropController.TransformSurface(position, Me.leftGrid, Me.DragSurface)
				If DropController.IsIn(Me.leftGrid, localPt) Then
					Return True
				End If
			End If

			Return False
		End Function

		Private Function CanAccept() As Boolean Implements IDropableObject.CanAccept
			Return True
		End Function

		Private Function GetThumbObject() As FrameworkElement Implements IDropableObject.GetThumbObject
			Return Nothing
		End Function

		Private Function GetThumbRect(ByVal position As Point) As Rect Implements IDropableObject.GetThumbRect
			Return New Rect()
		End Function

		#End Region
	End Class
End Namespace