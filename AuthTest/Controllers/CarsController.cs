using System.Security.Claims;
using AuthTest.Data;
using AuthTest.Dto;
using AuthTest.Extensions;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthTest.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CarsController(AppDbContext context, UserManager<User> userManager) : ControllerBase
{

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<Response<CarResponse>>>> GetCars(
        [FromQuery] PaginationParams paginationParams)
    {
        var query = context.Cars.Select(c => new CarResponse
        {
            Id = c.Id,
            Model = c.Model,
            PhotoUrl = c.PhotoUrl,
            UserId = c.Owner.Id
        });
        
        var totalItems = await query.CountAsync();
        
        var cars = await query
            .Skip((paginationParams.Page -1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToListAsync();
        
        return Ok(new Response<CarResponse>
        {
            Total = totalItems,
            Items = cars,
            Page = paginationParams.Page,
            PageSize = paginationParams.PageSize,
            TotalPages = (int) System.Math.Ceiling(totalItems / (double) paginationParams.PageSize)
        });
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Car>> GetCar(string id)
    {
        var car = await context.Cars.FindAsync(id);

        if (car is null)
        {
            return NotFound();
        }

        return car;
    }
    
    [HttpGet("user")]
    public async Task<ActionResult<IEnumerable<Car>>> GetUserCars()
    {
        var user = await userManager.GetUserAsync(HttpContext.User);

        if (user is null)
            return BadRequest("User not found");
        
        var cars = await context.Cars
            .Include(c => c.Owner)
            .Where(c => c.Owner.Id == user!.Id)
            .Select(c => new CarResponse
            {
                Id = c.Id,
                Model = c.Model,
                PhotoUrl = c.PhotoUrl,
                UserId = c.Owner.Id
            }).ToListAsync();

        return Ok(cars);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutCar(CarRequestForUpdate carRequest)
    {
        var car = await context.Cars
            .Where(c => c.Id == carRequest.Id)
            .Include(c => c.Owner)
            .FirstOrDefaultAsync();

        if (car is null)
            return BadRequest("Car not found");
        
        var userId = HttpContext.GetUserId();
        
        if (userId != car.Owner.Id)
        {
            return Unauthorized();
        }
        
        car.UpdateCar(carRequest.Model, carRequest.PhotoUrl);
        
        await context.SaveChangesAsync();
        
        return NoContent();
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Car>> PostCar(CarRequest carRequest)
    {
        var user = await userManager.GetUserAsync(HttpContext.User);

        var car = new Car
        {
            Id = Guid.NewGuid().ToString(),
            Model = carRequest.Model,
            PhotoUrl = carRequest.PhotoUrl,
            Owner = user!
        };
        
        context.Cars.Add(car);
        
        await context.SaveChangesAsync();

        return Ok(car);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteCar(string id)
    {
        var user = await userManager.GetUserAsync(HttpContext.User);
        
        var car = await context.Cars
            .Where(c => c.Id == id)
            .Include(c => c.Owner)
            .FirstOrDefaultAsync();
        
        if (car is null)
        {
            return NotFound();
        }

        context.Cars.Remove(car);
        await context.SaveChangesAsync();

        return NoContent();
    }
    
}