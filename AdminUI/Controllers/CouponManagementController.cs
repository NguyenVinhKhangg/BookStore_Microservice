using AdminUI.Models.Coupon;
using AdminUI.Services.CouponServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminUI.Controllers
{

    public class CouponManagementController : Controller
    {
        private readonly ICouponService _couponService;

        public CouponManagementController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        // GET: /CouponManagement
        public async Task<IActionResult> Index(string searchTerm, bool? statusFilter, decimal? discountFilter, int page = 1, int pageSize = 10)
        {
            try {
                var (coupons, totalCount) = await _couponService.GetAllAsync(searchTerm, statusFilter, discountFilter, page, pageSize);
                ViewBag.Filter = new CouponSearchFilterViewModel
                {
                    SearchTerm = searchTerm,
                    StatusFilter = statusFilter,
                    DiscountFilter = discountFilter
                };
                ViewBag.TotalCount = totalCount;
                ViewBag.CurrentPage = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                return View(coupons);
            }
            catch (HttpRequestException ex) {
                TempData["ErrorMessage"] = ex.Message;
                ViewBag.Filter = new CouponSearchFilterViewModel();
                ViewBag.TotalCount = 0;
                ViewBag.CurrentPage = 1;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalPages = 0;
                return View(Enumerable.Empty<CouponViewModel>());
            }
        }

        // GET: /CouponManagement/GetDetail/{id}
        public async Task<IActionResult> GetDetail(int id)
        {
            try {
                var coupon = await _couponService.GetByIdAsync(id);
                return PartialView("_CouponDetail", coupon);
            }
            catch (HttpRequestException ex) {
                return Content($"<div class='alert alert-danger'><i class='fas fa-exclamation-triangle'></i> {ex.Message}</div>");
            }
        }

        // GET: /CouponManagement/Create
        public IActionResult Create()
        {
            return View(new CouponViewModel
            {
                ValidFrom = DateTime.Today,
                ValidTo = DateTime.Today.AddDays(30),
                IsActive = true
            });
        }

        // POST: /CouponManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CouponViewModel viewModel)
        {
            if (ModelState.IsValid) {
                try {
                    await _couponService.CreateAsync(viewModel);
                    TempData["SuccessMessage"] = "Mã giảm giá đã được tạo thành công.";
                    return RedirectToAction(nameof(Index));
                }
                catch (HttpRequestException ex) {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(viewModel);
        }

        // GET: /CouponManagement/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            try {
                var coupon = await _couponService.GetByIdAsync(id);
                return View(coupon);
            }
            catch (HttpRequestException ex) {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /CouponManagement/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CouponViewModel viewModel)
        {
            if (id != viewModel.CouponID) {
                ModelState.AddModelError(string.Empty, "CouponID không khớp.");
                return View(viewModel);
            }

            if (ModelState.IsValid) {
                try {
                    await _couponService.UpdateAsync(id, viewModel);
                    TempData["SuccessMessage"] = "Mã giảm giá đã được cập nhật thành công.";
                    return RedirectToAction(nameof(Index));
                }
                catch (HttpRequestException ex) {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(viewModel);
        }

        // POST: /CouponManagement/Deactivate/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            try {
                await _couponService.DeactivateAsync(id);
                TempData["SuccessMessage"] = "Mã giảm giá đã được hủy kích hoạt.";
            }
            catch (HttpRequestException ex) {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: /CouponManagement/Activate/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int id)
        {
            try {
                await _couponService.ActivateAsync(id);
                TempData["SuccessMessage"] = "Mã giảm giá đã được kích hoạt.";
            }
            catch (HttpRequestException ex) {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
