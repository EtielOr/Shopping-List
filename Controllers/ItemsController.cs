using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingList.Data;
using ShoppingList.Models;

namespace ShoppingList.Controllers
{
    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ItemsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }



        // GET: Carts/5/Items
        [HttpGet("Carts/{cartId}/Items")]
        public async Task<IActionResult> Items(int? cartId)
        {
            if (cartId == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts.Include(c => c.Items)
                .FirstOrDefaultAsync(m => m.Id == cartId);

            if (cart == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);

            if (cart.OwnerId != userId)
            {
                return Unauthorized("No Unauthorized to view this data");
            }

            return View(cart);
        }


        [HttpPost("Carts/{cartId}/Items")]
        public async Task<IActionResult> AddItem(int? cartId, Item item)
        {
            if (cartId == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts.Include(c => c.Items)
                .FirstOrDefaultAsync(m => m.Id == cartId);

            if (cart == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);

            if (cart.OwnerId != userId)
            {
                return Unauthorized("No Unauthorized to view this data");
            }


            if (ModelState.IsValid)
            {
                try
                {
                    cart.Items.Add(item);

                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Redirect("/Carts/" + cartId + "/Items");
            }
            return Redirect("/Carts/" + cartId + "/Items");
        }

        // POST:  : Carts/CartStatus/5
        // change cart status
        [HttpPost("Carts/{cartId}/Items/ItemStatus/{itemId}")]
        public async Task<JsonResult> CartStatus(int cartId, int itemId)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(m => m.Id == cartId);

            var item = await _context.Items.FirstOrDefaultAsync(m => m.Id == itemId);

            var userId = _userManager.GetUserId(User);

            if (cart.OwnerId != userId || cart.Id != item.CartID)
            {
                return Json("No Unauthorized to edit this data");
            }

            item.Done = !item.Done;

            try
            {
                _context.Update(item);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                    throw;
            }
            return Json(cart.Done);
        }



        // POST: Item/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int cartId, int itemId)
        {
            var cart = await _context.Carts.FindAsync(cartId);

            var item = await _context.Items.FindAsync(itemId);

            var userId = _userManager.GetUserId(User);

            if (cart.OwnerId != userId)
            {
                return Unauthorized("No Unauthorized to view this data");
            }

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return Redirect("/Carts/" + cartId + "/Items");
        }



        private bool CartExists(int id)
        {
            return _context.Carts.Any(e => e.Id == id);
        }
    }
}