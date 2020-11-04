Namespace Logic
    Public Class RouteActions
        Sub RegisterCustomRoutes(routes As RouteCollection)
            routes.MapPageRoute(
                "ProductsByCategoryRoute",
                "Category/{categoryName}",
                "~/ProductList.aspx"
            )
            routes.MapPageRoute(
                "ProductByNameRoute",
                "Product/{productName}",
                "~/ProductDetails.aspx"
            )
        End Sub
    End Class
End Namespace
