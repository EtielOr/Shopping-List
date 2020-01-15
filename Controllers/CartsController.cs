using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoppingList.Data;
using ShoppingList.Models;

namespace ShoppingList
{
    [Authorize]
    public class CartsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CartsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: Carts
        public async Task<IActionResult> Index()
        {

            var userId = _userManager.GetUserId(User);

            var userCarts = from c in _context.Carts
                            where c.OwnerId == userId
                            select c;


            return View(await userCarts.ToListAsync());
        }

        // GET: Carts/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cart == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);

            if(cart.OwnerId != userId)
            {
                return Unauthorized("No Unauthorized to view this data");
            }

            return View(cart);
        }

        // GET: Carts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title")] Cart cart)
        {

            if (ModelState.IsValid)
            {
                cart.Done = false;
                cart.CreateDate = DateTime.UtcNow;
                cart.OwnerId = _userManager.GetUserId(User);
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cart);
        }

        // GET: Carts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts.FindAsync(id);

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

        // POST: Carts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Done")] Cart cart)
        {
            if (id != cart.Id)
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
                return RedirectToAction(nameof(Index));
            }
            return View(cart);
        }

        // GET: Carts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .FirstOrDefaultAsync(m => m.Id == id);

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

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cart = await _context.Carts.FindAsync(id);

            var userId = _userManager.GetUserId(User);

            if (cart.OwnerId != userId)
            {
                return Unauthorized("No Unauthorized to view this data");
            }

            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(int id)
        {
            return _context.Carts.Any(e => e.Id == id);
        }

        // POST:  : Carts/CartStatus/5
        // change cart status
        [HttpPost]
        public async Task<JsonResult> CartStatus(int id)
        {
            var cart = await _context.Carts.FindAsync(id);

            cart.Done = !cart.Done;

            var userId = _userManager.GetUserId(User);

            if (cart.OwnerId != userId)
            {
                return Json("No Unauthorized to edit this data");
            }

            try
            {
                _context.Update(cart);
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if (!CartExists(cart.Id))
                {
                   // return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Json(cart.Done);
        }
    }
}
