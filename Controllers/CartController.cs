using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cauman_Midterm_Store.Data;
using Cauman_Midterm_Store.Models;

namespace Cauman_Midterm_Store.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // READ - Show cart
        public async Task<IActionResult> Index()
        {
            var items = await _context.CartItems.ToListAsync();

            return View(items);
        }

        // ADD TO CART
        public async Task<IActionResult> Add(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.ProductId == id);

            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                var cartItem = new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = 1
                };

                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // UPDATE QUANTITY
        [HttpPost]
        public async Task<IActionResult> Update(int id, int quantity)
        {
            var item = await _context.CartItems.FindAsync(id);

            if (item != null)
            {
                if (quantity <= 0)
                {
                    _context.CartItems.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        // REMOVE FROM CART
        public async Task<IActionResult> Remove(int id)
        {
            var item = await _context.CartItems.FindAsync(id);

            if (item != null)
            {
                _context.CartItems.Remove(item);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}