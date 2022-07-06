using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AddressBook.Data;
using AddressBook.Models;
using AddressBook.Services.Interfaces;

namespace AddressBook.Controllers;

public class ContactsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IImageService _imageService;
    public ContactsController(ApplicationDbContext context, IImageService imageService)
    {
        _context = context;
        _imageService = imageService;
    }

    // GET: Contacts
    public async Task<IActionResult> Index()
    {
        return _context.Contacts != null ? 
            View(await _context.Contacts.ToListAsync()) :
            Problem("Entity set 'ApplicationDbContext.Contacts'  is null.");
    }

    // GET: Contacts/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.Contacts == null)
        {
            return NotFound();
        }

        var contact = await _context.Contacts
            .FirstOrDefaultAsync(m => m.Id == id);
        if (contact == null)
        {
            return NotFound();
        }

        return View(contact);
    }

    // GET: Contacts/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Contacts/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("FirstName,LastName,Address1,Address2,City,State,Zip,Email,Phone,Created,ImageData,ImageType,ImageFile,Id")] Contact contact)
    {
        if (ModelState.IsValid)
        {
            if (contact.ImageFile != null)
            {
                contact.ImageData = await _imageService.ConvertFileToByteArrayAsync(contact.ImageFile);
                contact.ImageType = contact.ImageFile.ContentType;
            }

            contact.Created = contact.Created.ToUniversalTime();
            _context.Add(contact);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(contact);
    }

    // GET: Contacts/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.Contacts == null)
        {
            return NotFound();
        }

        var contact = await _context.Contacts.FindAsync(id);
        if (contact == null)
        {
            return NotFound();
        }
        return View(contact);
    }

    // POST: Contacts/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("FirstName,LastName,Address1,Address2,City,State,Zip,Email,Phone,Created,ImageData,ImageType,Id")] Contact contact)
    {
        if (id != contact.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(contact);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactExists(contact.Id))
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
        return View(contact);
    }

    // GET: Contacts/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || _context.Contacts == null)
        {
            return NotFound();
        }

        var contact = await _context.Contacts
            .FirstOrDefaultAsync(m => m.Id == id);
        if (contact == null)
        {
            return NotFound();
        }

        return View(contact);
    }

    // POST: Contacts/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.Contacts == null)
        {
            return Problem("Entity set 'ApplicationDbContext.Contacts'  is null.");
        }
        var contact = await _context.Contacts.FindAsync(id);
        if (contact != null)
        {
            _context.Contacts.Remove(contact);
        }
            
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ContactExists(int id)
    {
        return (_context.Contacts?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}