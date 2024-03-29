﻿using MagicVilla_VillaApi.Data;
using MagicVilla_VillaApi.Models;
using MagicVilla_VillaApi.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaApi.Controllers
{
     [Route("api/VillaAPI")]
    //[Route("api/[controller]")]
    [ApiController]
    public class VillaAPIController:ControllerBase
    {
        // this will return multiple records 
        [HttpGet]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            return Ok(VillaStore.villaList);
        }

        // this will return only one record according id
        //[HttpGet("id")]
        // can even express this id actually integer

        [HttpGet("{id:int}",Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]


        // [ProducesResponseType(200, Type= typeof(VillaDTO))]
        // [ProducesResponseType(404)]
        // [ProducesResponseType(400)]
        public ActionResult<VillaDTO> GetVilla(int id)
        {
            if(id == 0)
            {
                return BadRequest();
            }
             var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            if(villa == null)
            {
                return NotFound();
            }
            return Ok(villa);
        }

        // ::::::::::::::::::::::::::POST:::::::::::::::::::::::



        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<VillaDTO> CreateVilla([FromBody]VillaDTO villaDTO)
        {
            //if(!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            // for name validations
            if (VillaStore.villaList.FirstOrDefault(u=>u.Name.ToLower() == villaDTO.Name.ToLower() )!= null){
                ModelState.AddModelError("CustomError", "VIlla already Exists!");
                return BadRequest(ModelState);
            }

            if(villaDTO== null)
            {
                return BadRequest(villaDTO);
            }
            if (villaDTO.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            villaDTO.Id  = VillaStore.villaList.OrderByDescending(u => u.Id).FirstOrDefault().Id +1;
            VillaStore.villaList.Add(villaDTO);

            return CreatedAtRoute("GetVilla",new { id = villaDTO.Id}, villaDTO);
        }

        // ::::::::::::::::::::::::::DELETE:::::::::::::::::::::::

        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteVilla(int id)
        {
            if(id == 0)
            {
                return BadRequest();
            };
            var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            VillaStore.villaList.Remove(villa);
            return NoContent();
        }


        // ::::::::::::::::::::::::::PUT/UPDATE:::::::::::::::::::::::
        [HttpPut ("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public IActionResult UpdateVilla(int id, [FromBody]VillaDTO villaDTO)
        {
            if(villaDTO == null || id != villaDTO.Id)
            {
                return BadRequest();
            }
            var villa =VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            villa.Name = villaDTO.Name;
            villa.Sqft = villaDTO.Sqft;
            villa.Occupancy= villaDTO.Occupancy;

            return NoContent();
        }


        // ::::::::::::::::::::::::::PATCH:::::::::::::::::::::::

        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public IActionResult UpdatePartialVilla(int id,JsonPatchDocument<VillaDTO> patchDTO)
        {
            if(patchDTO == null || id == 0)
            {
                return BadRequest();
            }

            var villa =VillaStore.villaList.FirstOrDefault(u=>u.Id == id);
            if(villa == null)
            {
                return BadRequest();
            }
            patchDTO.ApplyTo(villa, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }
    }
}
