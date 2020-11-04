Imports WingtipToys.Models

Namespace Logic
    Public Class AddProducts
        Public Function AddProduct(ProductName As String, ProductDesc As String, ProductPrice As String, ProductCategory As String, ProductImagePath As String) As Boolean
            Dim myProduct As New Product()
            myProduct.ProductName = ProductName
            myProduct.Description = ProductDesc
            myProduct.UnitPrice = Convert.ToDouble(ProductPrice)
            myProduct.ImagePath = ProductImagePath
            myProduct.CategoryID = Convert.ToInt32(ProductCategory)

            Using _db As New ProductContext()
                _db.Products.Add(myProduct)
                _db.SaveChanges()
            End Using

            Return True
        End Function
    End Class
End Namespace
