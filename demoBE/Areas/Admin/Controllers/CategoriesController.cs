using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using demoBE.Models;

namespace demoBE.Areas.Admin.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly MyStoreEntities db = new MyStoreEntities();

        public ActionResult Index()
        {
            return View(db.Category.OrderBy(category => category.CategoryName).ToList());
        }

        public ActionResult Details(int? id)
        {
            Category category = FindCategory(id);
            return category == null ? (ActionResult)HttpNotFound() : View(category);
        }

        public ActionResult Create()
        {
            return View(new Category());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CategoryID,CategoryName")] Category category)
        {
            ValidateCategory(category);

            if (!ModelState.IsValid)
            {
                return View(category);
            }

            db.Category.Add(category);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Đã thêm danh mục.";
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int? id)
        {
            Category category = FindCategory(id);
            return category == null ? (ActionResult)HttpNotFound() : View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CategoryID,CategoryName")] Category category)
        {
            ValidateCategory(category);

            if (!ModelState.IsValid)
            {
                return View(category);
            }

            Category existingCategory = db.Category.Find(category.CategoryID);
            if (existingCategory == null)
            {
                return HttpNotFound();
            }

            existingCategory.CategoryName = category.CategoryName;
            db.Entry(existingCategory).State = EntityState.Modified;
            db.SaveChanges();
            TempData["SuccessMessage"] = "Đã cập nhật danh mục.";
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int? id)
        {
            Category category = FindCategory(id);
            return category == null ? (ActionResult)HttpNotFound() : View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Category.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }

            if (db.Product.Any(product => product.CategoryID == id))
            {
                TempData["ErrorMessage"] = "Không thể xóa danh mục đang có sản phẩm tham chiếu.";
                return RedirectToAction("Index");
            }

            try
            {
                db.Category.Remove(category);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Đã xóa danh mục.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Không thể xóa danh mục vì dữ liệu đang được tham chiếu.";
            }

            return RedirectToAction("Index");
        }

        private Category FindCategory(int? id)
        {
            if (!id.HasValue)
            {
                return null;
            }

            return db.Category.Find(id.Value);
        }

        private void ValidateCategory(Category category)
        {
            if (category == null)
            {
                ModelState.AddModelError(string.Empty, "Dữ liệu danh mục không hợp lệ.");
                return;
            }

            category.CategoryName = (category.CategoryName ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(category.CategoryName))
            {
                ModelState.AddModelError("CategoryName", "Tên danh mục là bắt buộc.");
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
