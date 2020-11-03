Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports WingtipToys.Models

Namespace Logic
    Public Class ShoppingCartActions
        Implements IDisposable
        Public Property ShoppingCartId As String
        Private _db As ProductContext = New ProductContext()
        Public Const CartSessionKey As String = "CartId"

        Public Sub AddToCart(id As Integer)
            ShoppingCartId = GetCartId()

            Dim cartItem = _db.ShoppingCartItems.SingleOrDefault(
                Function(c) c.CartId = ShoppingCartId AndAlso c.ProductId = id)

            If cartItem Is Nothing Then
                cartItem = New CartItem With {
                    .ItemId = Guid.NewGuid().ToString(),
                    .ProductId = id,
                    .CartId = ShoppingCartId,
                    .Product = _db.Products.SingleOrDefault(
                        Function(p) p.ProductID = id),
                    .Quantity = 1,
                    .DateCreated = DateTime.Now
                }

                _db.ShoppingCartItems.Add(cartItem)
            Else
                cartItem.Quantity += 1
            End If
            _db.SaveChanges()
        End Sub

        Public Sub IDisposable_Dispose() Implements IDisposable.Dispose
            If _db IsNot Nothing Then
                _db.Dispose()
                _db = Nothing
            End If
        End Sub

        Public Function GetCartId() As String
            If HttpContext.Current.Session(CartSessionKey) Is Nothing Then
                If Not String.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) Then
                    HttpContext.Current.Session(CartSessionKey) = HttpContext.Current.User.Identity.Name
                Else
                    Dim tempCartId As Guid = Guid.NewGuid()
                    HttpContext.Current.Session(CartSessionKey) = tempCartId.ToString()
                End If
            End If
            Return HttpContext.Current.Session(CartSessionKey).ToString()
        End Function

        Public Function GetCartItems() As List(Of CartItem)
            ShoppingCartId = GetCartId()

            Return _db.ShoppingCartItems.Where(
                Function(c) c.CartId = ShoppingCartId).ToList()
        End Function

        Public Function GetTotal() As Decimal
            ShoppingCartId = GetCartId()
            Dim total As Decimal? = Decimal.Zero
            total = CType((From cartItems In _db.ShoppingCartItems Where cartItems.CartId = ShoppingCartId
                           Select CType((cartItems.Quantity * cartItems.Product.UnitPrice), Integer?)).Sum(), Decimal?)
            Return If(total, Decimal.Zero)
        End Function

        Public Function GetCart(context As HttpContext) As ShoppingCartActions
            Using cart As New ShoppingCartActions()
                cart.ShoppingCartId = cart.GetCartId()
                Return cart
            End Using
        End Function

        Public Sub UpdateShoppingCartDatabase(cartId As String, CartItemUpdates As ShoppingCartUpdates())
            Using db As New WingtipToys.Models.ProductContext()
                Try
                    Dim CartItemCount As Integer = CartItemUpdates.Count()
                    Dim myCart As List(Of CartItem) = GetCartItems()
                    For Each cartItem In myCart
                        For i As Integer = 0 To CartItemCount - 1
                            If cartItem.Product.ProductID = CartItemUpdates(i).ProductId Then
                                If CartItemUpdates(i).PurchaseQuantity < 1 OrElse CartItemUpdates(i).RemoveItem = True Then
                                    RemoveItem(cartId, cartItem.ProductId)
                                Else
                                    UpdateItem(cartId, cartItem.ProductId, CartItemUpdates(i).PurchaseQuantity)
                                End If
                            End If
                        Next
                    Next
                Catch exp As Exception
                    Throw New Exception("ERROR: Unable to Update Cart Database - " + exp.Message.ToString(), exp)
                End Try
            End Using
        End Sub

        Public Sub RemoveItem(removeCartID As String, removeProductID As Integer)
            Using _db As New WingtipToys.Models.ProductContext()
                Try
                    Dim myItem = (From c In _db.ShoppingCartItems Where c.CartId = removeCartID AndAlso c.ProductId = removeProductID Select c).FirstOrDefault()
                    If myItem IsNot Nothing Then
                        _db.ShoppingCartItems.Remove(myItem)
                        _db.SaveChanges()
                    End If
                Catch exp As Exception
                    Throw New Exception("ERROR: Unable to Remove Cart Item - " + exp.Message.ToString(), exp)
                End Try
            End Using
        End Sub

        Public Sub UpdateItem(updateCartID As String, updateProductID As Integer, quantity As Integer)
            Using _db As New WingtipToys.Models.ProductContext()
                Try
                    Dim myItem = (From c In _db.ShoppingCartItems Where c.CartId = updateCartID AndAlso c.Product.ProductID = updateProductID Select c).FirstOrDefault()
                    If myItem IsNot Nothing Then
                        myItem.Quantity = quantity
                        _db.SaveChanges()
                    End If
                Catch exp As Exception
                    Throw New Exception("ERROR: Unable to Update Cart Item - " + exp.Message.ToString(), exp)
                End Try
            End Using
        End Sub

        Public Sub EmptyCart()
            ShoppingCartId = GetCartId()
            Dim cartItems = _db.ShoppingCartItems.Where(Function(c) c.CartId = ShoppingCartId)
            For Each cartItem In cartItems
                _db.ShoppingCartItems.Remove(cartItem)
            Next
            _db.SaveChanges()
        End Sub

        Public Function GetCount() As Integer
            ShoppingCartId = GetCartId()
            Dim count As Integer? = (From cartItems In _db.ShoppingCartItems Where cartItems.CartId = ShoppingCartId Select CType(cartItems.Quantity, Integer?)).Sum()
            Return If(count, 0)
        End Function

        Public Structure ShoppingCartUpdates
            Public ProductId As Integer
            Public PurchaseQuantity As Integer
            Public RemoveItem As Boolean
        End Structure

        Public Sub MigrateCart(cartId As String, userName As String)
            Dim shoppingCart = _db.ShoppingCartItems.Where(Function(c) c.CartId = cartId)
            For Each item As CartItem In shoppingCart
                item.CartId = userName
            Next
            HttpContext.Current.Session(CartSessionKey) = userName
            _db.SaveChanges()
        End Sub
    End Class
End Namespace

