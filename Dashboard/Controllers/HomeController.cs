using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Dashboard.Models;
using Microsoft.EntityFrameworkCore;
using Dashboard.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;

namespace Dashboard.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public HomeController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    #region General

    [Authorize]
    public IActionResult Index()
    {
        var username = HttpContext.User.Identity.Name ?? null;
        HttpContext.Session.SetString("userdata", username);
        ViewBag.Username = username;
        return View();
    }
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    #endregion


    #region Products

    [Route("create")]
    public IActionResult Create(Products product)
    {
        try
        {
            _context.products.Add(product);
            _context.SaveChanges();
            TempData["add"] = true;
            return RedirectToAction("Index");
        }
        catch
        {
            TempData["add"] = false;
            var products = _context.products.ToList();
            ViewBag.Product = products;
            return View("Index");
        }
    }


    public IActionResult Edit(int id)
    {
        var product = _context.products.SingleOrDefault(x => x.Id == id);
        return View(product);
    }

    public IActionResult Update(Products product)
    {
        if (ModelState.IsValid)
        {
            _context.products.Update(product);
            _context.SaveChanges();
        }
        return RedirectToAction("Index");
    }



    public IActionResult Delete(int id)
    {
        var product = _context.products.SingleOrDefault(x => x.Id == id);
        if (product != null)
        {
            _context.products.Remove(product);
            _context.SaveChanges();
            TempData["del"] = true;
        }
        else
        {
            TempData["del"] = false;
        }
        return RedirectToAction("Index");
    }

    public IActionResult Indexp()
    {
        var products = _context.products.ToList();
        ViewBag.Product = products;
        return View();
    }

    public IActionResult Addnewitems()
    {
        ViewBag.Username = HttpContext.Session.GetString("userdata");
        var products = _context.products.ToList();
        ViewBag.Products = products;
        return View();
    }

    #endregion


    #region ProductDetails


    public IActionResult CreateDetails()
    {
        return View();
    }



    public IActionResult CreateDeatils(ProductsDetails pd, IFormFile Images)
    {
        try
        {
            if (Images != null && Images.Length > 0)
            {
                var imgPath = Path.Combine(_webHostEnvironment.WebRootPath, "img");
                if (!Directory.Exists(imgPath))
                {
                    Directory.CreateDirectory(imgPath);
                }

                var path = Path.Combine(imgPath, Images.FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    Images.CopyTo(stream);
                }
                pd.Images = Images.FileName;
            }
            else
            {
                ModelState.AddModelError("", "يجب اختيار صورة.");
                return View("ProductsDetails", pd);
            }

            if (ModelState.IsValid)
            {
                _context.productsDetails.Add(pd);
                _context.SaveChanges();
                return RedirectToAction("ProductDetails");
            }
            else
            {
                return View("ProductsDetails", pd);
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"حدث خطأ أثناء إضافة المنتج: {ex.Message}");
            return View("ProductsDetails", pd);
        }
    }

    public IActionResult EditD(int product)
    {
        var products = _context.productsDetails.SingleOrDefault(x => x.Id == product);
        var productn = _context.products.SingleOrDefault(x => x.Id == product);
        if (productn != null)
        {
            ViewData["name"] = productn.Name.ToString();
        }
        else
        {
            ViewData["name"] = "none";
        }
        var produc = _context.products.ToList();
        ViewBag.Products = produc;
        return View(products);
    }

    public IActionResult UpdateD(ProductsDetails product, IFormFile Images)
    {
        var existingProduct = _context.productsDetails.Find(product.Id);

        if (existingProduct == null)
        {
            return NotFound();
        }

        if (Images != null && Images.Length > 0)
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "img", Images.FileName);
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                Images.CopyTo(stream);
            }
            existingProduct.Images = Images.FileName;
        }

        if (!string.IsNullOrEmpty(product.Color))
        {
            existingProduct.Color = product.Color;
        }

        if (product.Qty > 0)
        {
            existingProduct.Qty = product.Qty;
        }

        if (product.Price > 0)
        {
            existingProduct.Price = product.Price;
        }

        existingProduct.ProductId = product.ProductId;

        _context.productsDetails.Update(existingProduct);
        _context.SaveChanges();

        return RedirectToAction("ProductDetails");
    }


    public IActionResult DeleteD(int productdet)
    {
        var productDetails = _context.productsDetails.SingleOrDefault(dp => dp.Id == productdet);
        if (productDetails != null)
        {
            _context.productsDetails.Remove(productDetails);
            _context.SaveChanges();
        }
        return RedirectToAction("ProductDetails");
    }


    public IActionResult ProductDetails()
    {
        var productD = _context.productsDetails.Join(
            _context.products,
            x => x.ProductId,
            y => y.Id,
            (x, y) => new
            {
                id = x.Id,
                name = y.Name,
                qty = x.Qty,
                price = x.Price,
                images = x.Images,
                color = x.Color,
            }
        ).ToList();

        ViewBag.ProductD = productD;

        var products = _context.products.ToList();
        ViewBag.Products = products;

        return View(); 
    }




    public IActionResult CreateProducts(Products products)
    {
        _context.Add(products);
        _context.SaveChanges();
        return RedirectToAction("Addnewitems");
    }


    public IActionResult ProductsDetails()
    {
        var products = _context.products.ToList();
        ViewBag.Products = products;

        var productDetails = _context.productsDetails.ToList();
        return View(productDetails);
    }
    #endregion



    #region DamagedProducts


    public IActionResult AddDemag(Damegedproducts damege)
    {
        if (ModelState.IsValid)
        {
            _context.Add(damege);
            _context.SaveChanges();
            return RedirectToAction("Demag");
        }
        return View(damege);
    }

    public IActionResult Demag()
    {
        ViewBag.Username = HttpContext.Session.GetString("userdata");
        var products = _context.products.ToList();

        var Productsdemage = _context.damegedproducts
            .Join(_context.products,
                  demag => demag.ProductId,
                  product => product.Id,
                  (demag, product) => new { demag, product })
            .Join(_context.productsDetails,
                  p => p.demag.ProductId,
                  detail => detail.ProductId,
                  (p, detail) => new
                  {
                      id = p.demag.Id,
                      name = p.product.Name,
                      color = detail.Color,
                      qty = p.demag.Qty
                  })
            .ToList();

        ViewBag.products = products;
        ViewBag.damage = Productsdemage;

        return View();
    }

    public IActionResult ShowProductsDetails()
    {
        ViewBag.Username = HttpContext.Session.GetString("userdata");
        var products = _context.products.ToList();
        ViewBag.Products = products;

        var productsDetails = _context.productsDetails.ToList();
        ViewBag.ProductsDetails = productsDetails;

        return View("ProductsDetails");
    }

    #endregion


}
