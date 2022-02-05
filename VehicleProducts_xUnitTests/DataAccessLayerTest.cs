﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore; 
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleProducts.Db;
using VehicleProducts.Models;
using VehicleProducts_xUnitTests.Services;
using Xunit;
using VehicleProducts.Controllers;
using Microsoft.AspNetCore.Mvc;

#nullable disable

namespace VehicleProducts_xUnitTests
{
    public class DataAccessLayerTest
    {

        [Fact]
        public async Task AddProductAsync_ProductIsAdded()
        {
            using (var db = new ProductDbContext(DatabaseService.TestDbContextOptions()))
            {
                //// Arrange 
                var product = new VehicleModel()
                {
                    Title = "Test",
                    ProductDescription = "Test Description"
                }; 

                //// Act 
                //await db.AddAsync(product);
                //await db.SaveChangesAsync();
                await db.AddProductAsync(product);

                // Getting the recently inserted ID because it's auto generated by our db.
                // *** Note: this line must come after SaveChanges() or SaveChangesAsync() methods. 
                int id = product.Id;

                //// Assert
                var insertedProduct = await db.FindAsync<VehicleModel>(id); 
                Assert.Equal(product, insertedProduct);


            }// end using 
        }// end AddProductAsync_ProductIsAdded()

        [Fact]
        public async Task EditedProductAsync_ProductIsEdited()
        {
            using (var db = new ProductDbContext(DatabaseService.TestDbContextOptions()))
            {
                //// Arrange 
                var product = new VehicleModel()
                {
                    Title = "Test",
                    ProductDescription = "Test Description"
                };

                //// Act 
                //await db.AddAsync(product);
                //await db.SaveChangesAsync();
                await db.AddProductAsync(product); 

                // Getting the recently inserted ID because it's auto generated by our db.
                // *** Note: this line must come after SaveChanges() or SaveChangesAsync() methods. 
                int id = product.Id;

                //// *** Editing product 
                var editedTitle = "Test Edited";
                var editedDescription = "Test description edited"; 

                var editedProduct = await db.FindAsync<VehicleModel>(id);
                editedProduct.Title = editedTitle;
                editedProduct.ProductDescription = editedDescription;

                //db.Update<VehicleModel>(editedProduct); 
                //await db.SaveChangesAsync();
                await db.UpdateProductAsync(editedProduct);



                //// Assert
                var getEditedProduct = await db.FindAsync<VehicleModel>(id); 

                Assert.Equal(editedTitle, getEditedProduct.Title);
                Assert.Equal(editedDescription, getEditedProduct.ProductDescription);


            }// end using 
        }// end EditedProductAsync_ProductIsEdited()

        [Fact]
        public async Task AddingProductListAsync_ProductListIsAdded()
        {
            //// Creatting new list
            //List<VehicleModel> DummyMemoryTestList = new List<VehicleModel>()
            //{
            //    new VehicleModel() { Title = "Test 1", ProductDescription = "Test 1 Description"},
            //    new VehicleModel() { Title = "Test 2", ProductDescription = "Test 2 Description"},
            //    new VehicleModel() { Title = "Test 3", ProductDescription = "Test 3 Description"},
            //    new VehicleModel() { Title = "Test 4", ProductDescription = "Test 4 Description"},
            //    new VehicleModel() { Title = "Test 5", ProductDescription = "Test 5 Description"},
            //}; 

            using (var db = new ProductDbContext(DatabaseService.TestDbContextOptions()))
            {
                //// Arrange 
                await db.Vehicles.AddRangeAsync(DatabaseService.DummyMemoryTestList);
                await db.SaveChangesAsync();

                //// Act 
                var actualListCount = DatabaseService.DummyMemoryTestList.Count();
                var expectedList = await db.Vehicles.ToListAsync();
                var expectedListCount = expectedList.Count;


                //// Assert
                Assert.Equal(actualListCount, expectedListCount);

            }// end using 
        }// end EditedProductAsync_ProductIsEdited()

        [Fact]
        public async Task DeleteProductAsync_ProductIsDeleted()
        {
            

            using (var db = new ProductDbContext(DatabaseService.TestDbContextOptions()))
            {
                //// Arrange 
                await db.Vehicles.AddRangeAsync(DatabaseService.DummyMemoryTestList);
                await db.SaveChangesAsync();

                //// Act 
                var actualList = db.Vehicles.Where(i => i.Title != DatabaseService.DummyMemoryTestList[0].Title);
               

                try
                {
                    var productToDelete = db.Vehicles.Where(i => i.Title == DatabaseService.DummyMemoryTestList[0].Title).FirstOrDefault();
                    await db.DeleteProductAsync(productToDelete); 
                }
                catch (Exception ex)
                {

                }// end try 

                var expectedList = await db.Vehicles.AsNoTracking().ToListAsync();


                //// Assert
                //// Checking if two lists are equal. 
                Assert.Equal(
                                actualList.OrderBy(p => p.Id).Select(p => p.Title),
                                expectedList.OrderBy(p => p.Id).Select(p => p.Title)
                            ); 

            }// end using 


        }// end DeleteProductAsync_ProductIsDeleted()

        

    }// end class DataAccessLayerTest
}
