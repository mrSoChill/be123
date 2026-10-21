# Audit EF6 Database First — Model1.edmx

Audit date: 2026-10-21 (Asia/Ho_Chi_Minh)

## Provenance and project integration

Visual Studio EF Designer generated `Models/Model1.edmx`, its `.diagram`, T4
templates, `Model1.Context.cs`, and entity files. The generated model is named
`Model1.edmx` rather than `MyStoreModel.edmx`; it is retained as generated to
avoid breaking the Designer-generated metadata resource names. No EDMX XML or
generated entity class was authored manually.

The legacy MVC project file was missing entries for the newly generated files.
It now includes the generated C# sources, T4 inputs and `EntityDeploy` item for
`Model1.edmx`. This embeds the `Models.Model1.csdl`, `.ssdl`, and `.msl`
resources required by the connection string. IIS Express queried MyStore
successfully through the context, which verifies this integration at runtime.

The project `Web.config` also removes the obsolete `.edmx` build provider
inherited from .NET Framework machine.config. EF6 6.5.1 embeds metadata via
`EntityDeploy`; without this override, `MvcBuildViews=true` fails while trying
to load the obsolete `System.Data.Entity.Design.AspNet.EntityDesignerBuildProvider`.

## Context and connection

| Item | Verified value |
| --- | --- |
| DbContext | `demoBE.Models.MyStoreEntities` |
| Base connection name | `name=MyStoreEntities` |
| Entity sets | `Category`, `Customer`, `Order`, `OrderDetail`, `Product`, `User` |
| Provider | `System.Data.SqlClient` through `System.Data.EntityClient` |
| Database | `MyStore` on `(localdb)\MSSQLLocalDB` with Windows Integrated Security |
| Metadata resources | `res://*/Models.Model1.csdl`, `.ssdl`, `.msl` |
| Credentials | None stored; connection string contains no password/token. |

## Entities, keys, and navigation properties

| Entity | Key | Relevant navigation |
| --- | --- | --- |
| `Category` | `CategoryID` | `ICollection<Product> Product` |
| `Product` | `ProductID` | `Category Category`, `ICollection<OrderDetail> OrderDetail` |
| `User` | `Username` | `ICollection<Customer> Customer` |
| `Customer` | `CustomerID` | `User User`, `ICollection<Order> Order` |
| `Order` | `OrderID` | `Customer Customer`, `ICollection<OrderDetail> OrderDetail` |
| `OrderDetail` | `ID` | `Order Order`, `Product Product` |

`ProductDecription` is mapped as a required `string` on `Product`, preserving
the exact source-database spelling. The five mappings are Customer→User,
Product→Category, Order→Customer, OrderDetail→Order, and OrderDetail→Product.
They match the foreign keys verified in SQL Server.

## Runtime verification

IIS Express used the configured connection string to load both
`/Admin/Categories/Index` and `/Admin/Products/Index` with HTTP 200. Product
Index executed `Include(product => product.Category)` and rendered the linked
Category name during the CRUD test, verifying the generated Product–Category
navigation property and FK mapping.
