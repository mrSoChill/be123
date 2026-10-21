using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using demoBE.Models;

namespace demoBE.Areas.Admin.Controllers
{
    public class ProductsController : Controller
    {
        private readonly MyStoreEntities db = new MyStoreEntities();

        public ActionResult Index()
        {
            return View(db.Product
                .Include(product => product.Category)
                .OrderBy(product => product.ProductName)
                .ToList());
        }

        public ActionResult Details(int? id)
        {
            Product product = FindProduct(id);
            return product == null ? (ActionResult)HttpNotFound() : View(product);
        }

        public ActionResult Create()
        {
            PopulateCategories();
            return View(new Product());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,CategoryID,ProductName,ProductDecription,ProductPrice,ProductImage")] Product product)
        {
            ValidateProduct(product);
            if (!ModelState.IsValid)
            {
                PopulateCategories(product == null ? (int?)null : product.CategoryID);
                return View(product);
            }

            db.Product.Add(product);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Đã thêm sản phẩm.";
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int? id)
        {
            Product product = FindProduct(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            PopulateCategories(product.CategoryID);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,CategoryID,ProductName,ProductDecription,ProductPrice,ProductImage")] Product product)
        {
            ValidateProduct(product);
            if (!ModelState.IsValid)
            {
                PopulateCategories(product == null ? (int?)null : product.CategoryID);
                return View(product);
            }

            Product existingProduct = db.Product.Find(product.ProductID);
            if (existingProduct == null)
            {
                return HttpNotFound();
            }

            existingProduct.CategoryID = product.CategoryID;
            existingProduct.ProductName = product.ProductName;
            existingProduct.ProductDecription = product.ProductDecription;
            existingProduct.ProductPrice = product.ProductPrice;
            existingProduct.ProductImage = product.ProductImage;
            db.Entry(existingProduct).State = EntityState.Modified;
            db.SaveChanges();
            TempData["SuccessMessage"] = "Đã cập nhật sản phẩm.";
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int? id)
        {
            Product product = FindProduct(id);
            return product == null ? (ActionResult)HttpNotFound() : View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            if (db.OrderDetail.Any(detail => detail.ProductID == id))
            {
                TempData["ErrorMessage"] = "Không thể xóa sản phẩm đã có chi tiết đơn hàng tham chiếu.";
                return RedirectToAction("Index");
            }

            try
            {
                db.Product.Remove(product);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Đã xóa sản phẩm.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Không thể xóa sản phẩm vì dữ liệu đang được tham chiếu.";
            }

            return RedirectToAction("Index");
        }

        private Product FindProduct(int? id)
        {
            if (!id.HasValue)
            {
                return null;
            }

            return db.Product.Include(product => product.Category)
                .SingleOrDefault(product => product.ProductID == id.Value);
        }

        private void PopulateCategories(int? selectedCategoryId = null)
        {
            ViewBag.CategoryID = new SelectList(
                db.Category.OrderBy(category => category.CategoryName).ToList(),
                "CategoryID",
                "CategoryName",
                selectedCategoryId);
        }

        private void ValidateProduct(Product product)
        {
            if (product == null)
            {
                ModelState.AddModelError(string.Empty, "Dữ liệu sản phẩm không hợp lệ.");
                return;
            }

            product.ProductName = (product.ProductName ?? string.Empty).Trim();
            product.ProductDecription = (product.ProductDecription ?? string.Empty).Trim();
            product.ProductImage = string.IsNullOrWhiteSpace(product.ProductImage) ? null : product.ProductImage.Trim();

            if (string.IsNullOrWhiteSpace(product.ProductName))
            {
                ModelState.AddModelError("ProductName", "Tên sản phẩm là bắt buộc.");
            }

            if (string.IsNullOrWhiteSpace(product.ProductDecription))
            {
                ModelState.AddModelError("ProductDecription", "Mô tả sản phẩm là bắt buộc.");
            }

            if (product.ProductPrice <= 0)
            {
                ModelState.AddModelError("ProductPrice", "Giá sản phẩm phải lớn hơn 0.");
            }

            if (!db.Category.Any(category => category.CategoryID == product.CategoryID))
            {
                ModelState.AddModelError("CategoryID", "Vui lòng chọn danh mục hợp lệ.");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
