﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Movies.API.Filters;
using Movies.Business;
using Movies.Business.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Movies.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GenresController : ControllerBase
    {

        private IGenreService service;
        public GenresController(IGenreService genreService)
        {
            service = genreService;
        }

        [HttpGet]
        [AllowAnonymous]
        [ResponseCache(Duration =300)]
        public IActionResult Get()
        {
            var result = service.GetAllGenres();
            return Ok(new
            {
                genres = result,
                value = DateTime.Now.ToString()
            }) ;
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [ResponseCache(CacheProfileName ="list")]
        
        //[ResponseCache(Duration =300, VaryByQueryKeys =new[] { "id" })]
        public IActionResult GetById(int id)
        {
            var genreListReponse = service.GetGenresById(id);
            if (genreListReponse != null)
            {
                return Ok(new { result = genreListReponse, cacheTime = DateTime.Now.ToString() });
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles ="Admin,Editor")]
        public IActionResult AddGenre(AddNewGenreRequest request)
        {
            if (ModelState.IsValid)
            {
                int genreId = service.AddGenre(request);
                return CreatedAtAction(nameof(GetById), routeValues: new { id = genreId }, value: null);
            }

            return BadRequest(ModelState);

        }
        [HttpPut("{id}")]
        [GenreExists]
        public IActionResult UpdateGenre(int id, EditGenreRequest request)
        {
            //Aşağıdaki işlemi, [GenreExists] yapıyor. Detaylar; Filters/GenreExistsAttribute.cs'de:
            //var isExisting = service.GetGenresById(id);
            //if (isExisting == null)
            //{
            //    return NotFound();
            //}

            if (ModelState.IsValid)
            {
                int newItemId = service.UpdateGenre(request);
                return Ok();
            }
            return BadRequest(ModelState);
        }
        [HttpDelete("{id}")]
        [GenreExists]
        public IActionResult Delete(int id)
        {
            service.DeleteGenre(id);
            return Ok();
        }


    }
}
