using IntegrationTest.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using VehicleProducts;
using VehicleProducts.Db;
using VehicleProducts.Models;
using VehicleProducts.ViewModels;
using Xunit;
using static System.Net.Mime.MediaTypeNames;

#nullable disable

/// <summary>
/// Articel To Read: https://auth0.com/blog/xunit-to-test-csharp-code/
/// 
/// Articel to Read: Asp.Net Core Integration Testing, publication year 2022 https://code-maze.com/aspnet-core-integration-testing/ 
/// <see cref="https://github.com/CodeMazeBlog/testing-aspnetcore-mvc/tree/integration-testing-mvc"/>
/// 
/// </summary>
namespace IntegrationTest
{
    public class IntegrationTests_AdminController : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory; 
     

        /// <summary>
        /// <see cref="IntegrationTests_AdminController"/> will perform Integration Test. 
        /// We will test all CRUD operations using HTTP GET and POST using <see cref="HttpClient"/>. 
        /// </summary>
        /// <param name="factory">Custom Web Application Factory which will perform in-memory database.</param>
        public IntegrationTests_AdminController(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;

        }// end AdminControllerTests()


        #region Test Sample 

        /// <summary>
        /// <see cref="WebApplicationFactory_SampleTest"/> is the sample test factory. 
        /// If you don't need <see cref="CustomWebApplicationFactory{T}"/>, you may use as <see cref="WebApplicationFactory_SampleTest"/>. 
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task WebApplicationFactory_SampleTest()
        {
            var application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(webHostBuilder =>
                {
                    //// Configure test services 
                    webHostBuilder.ConfigureServices(async services =>
                    {
                    var dbContext = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ProductDbContext>));

                    if (dbContext != null) services.Remove(dbContext);

                    //// Adding Dependency Injection and Injection InMemory Db
                    services.AddDbContext<ProductDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("VehicelInMemoryDbForTesting");
                    });

                    var sp = services.BuildServiceProvider();

                    using (var scope = sp.CreateScope())
                    {
                        var scopedServices = scope.ServiceProvider;
                        var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<Program>>>();


                        var db = scopedServices.GetRequiredService<ProductDbContext>();


                        try
                        {
                            db.Database.EnsureCreated();

                            await db.AddRangeAsync(DatabaseUtilitiesService.DummyMemoryTestList);
                            await db.SaveChangesAsync();
                                
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "An error occurred seeding the " + "database with test messages. Error: {Message}", ex.Message);
                        }// end try 

                            string test = ""; 

                        }// end using 
                    }); 
                });


            var client = application.CreateClient();

            var response = await client.GetAsync("/home/index");


        }// end Test1()

        [Fact]
        public async Task Home_Index_Test()
        {
            // Arrange 
            var _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });


            // Act 
            var response = await _client.GetAsync("/home/index");


            // Assert 
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

        }// end Home_Index_Test()

        /// <summary>
        /// This method will verify that we can perform Integration Testing. 
        /// </summary>
        /// <param name="url">URL Name which is passed by InlineData</param>
        /// <returns></returns>
        [Theory]
        [InlineData("/")]
        [InlineData("/Home/Index")]
        [InlineData("/Home/Privacy")]
        public async Task Home_Controller_HTTP_Test(string url)
        {
            // Arragne 
            var _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });

            // Act 
            var response = await _client.GetAsync(url); 

            // Assert 
            response.EnsureSuccessStatusCode();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());


        }// end Home_Index_Test()

        #endregion

        #region Authentication and Authorization and CRUD Operations

        [Fact]
        public async Task Get_SecurePageRedirectsAnUnauthenticatedUser()
        {
            // Arragne => Done at constructor 
            var _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });

            // Act 
            var response = await _client.GetAsync("/Admin"); 

            // Assert 

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.StartsWith("http://localhost/Identity/Account/Login", response.Headers.Location.OriginalString); 
            
        }// end Get_SecurePageRedirectsAnUnauthenticatedUser()

        /// <summary>
        /// <see cref="Get_IndexAsAnAuthenticated_AdminUser"/> as login admin user. 
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Get_IndexAsAnAuthenticated_AdminUser()
        {
            // Arrange 
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddAuthentication("Admin")
                            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Admin", options => { })
                            ;
                });

            })
            .CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });

            // Act 
            var response = await client.GetAsync("/Admin/Index");

            // Assert 
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        }// end Get_IndexAsAnAuthenticated_AdminUser()


        [Fact]
        public async Task AddProduct_HTTP_GET_AsAnAuthenticated_AdminUser()
        {
            // Arrange 
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddAuthentication("Admin")
                            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Admin", options => { })
                            ;
                });

            })
            .CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });

            // Act 
            var response = await client.GetAsync("/Admin/Add");
            var getAddPageData = await response.Content.ReadAsStringAsync(); 

            // Assert 
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("Admin Page - Add", getAddPageData);

        }// end AddProduct_HTTP_GETAsAnAuthenticated_AdminUser()

        
        [Fact]
        public async Task AdminRedirectTest_HttpPost_AsAnAuthenticated_AdminUser_AllowAutoRedirect_IS_False()
        {


            // Arrange 
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddAuthentication("Admin")
                            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Admin", options => { })
                            ;
                });

            })
            .CreateClient(new WebApplicationFactoryClientOptions() { AllowAutoRedirect = false });


            // Act 
            var request = await client.GetAsync("/Admin/RedirectTest");


            var httpHeader = request.Headers.ToString();
            var reqContent = await request.Content.ReadAsStringAsync();

            // Getting Cookie Value from HTTP Header 
            var antiforgeryCookieValue = AntiForgeryTokenExtractor.ExtractCookieValue(httpHeader);
            var antiForgeryVal = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(antiforgeryCookieValue, reqContent);


            var httpPostReq = new HttpRequestMessage(HttpMethod.Post, "/Admin/RedirectTest");
            httpPostReq.Headers.Add("Cookie", new CookieHeaderValue(AntiForgeryTokenExtractor.Cookie, antiforgeryCookieValue).ToString());

            var resposne = await client.SendAsync(httpPostReq);

            // Assert 
            Assert.Equal(HttpStatusCode.OK, request.StatusCode);


        }// end AddProduct_HTTP_POST_ValidModel_AsAnAuthenticated_AdminUser()


        [Fact]
        public async Task AdminRedirectTest_HttpPost_AsAnAuthenticated_AdminUser_AllowAutoRedirect_IS_TRUE()
        {


            // Arrange 
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddAuthentication("Admin")
                            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Admin", options => { })
                            ;
                });

            })
            .CreateClient(new WebApplicationFactoryClientOptions() { AllowAutoRedirect = true });


            // Act 
            var request = await client.GetAsync("/Admin/RedirectTest");


            var httpHeader = request.Headers.ToString();
            var reqContent = await request.Content.ReadAsStringAsync();

            // Getting Cookie Value from HTTP Header 
            var antiforgeryCookieValue = AntiForgeryTokenExtractor.ExtractCookieValue(httpHeader);
            var antiForgeryVal = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(antiforgeryCookieValue, reqContent);


            var httpPostReq = new HttpRequestMessage(HttpMethod.Post, "/Admin/RedirectTest");
            httpPostReq.Headers.Add("Cookie", new CookieHeaderValue(AntiForgeryTokenExtractor.Cookie, antiforgeryCookieValue).ToString());

            var resposne = await client.SendAsync(httpPostReq);

            // Assert 
            Assert.Equal(HttpStatusCode.OK, request.StatusCode);


        }// end AddProduct_HTTP_POST_ValidModel_AsAnAuthenticated_AdminUser()

        [Fact]
        public async Task AddProduct_HTTP_POST_ValidModel_AsAnAuthenticated_AdminUser_AllowAutoRedirect_IS_False()
        {


            // Arrange 
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddAuthentication("Admin")
                            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Admin", options => { })
                            ;
                });

            })
            .CreateClient(new WebApplicationFactoryClientOptions() { AllowAutoRedirect = false });


            // Act 
            var request = await client.GetAsync("/Admin/Add");


            var httpHeader = request.Headers.ToString();
            var reqContent = await request.Content.ReadAsStringAsync();

            // Getting Cookie Value from HTTP Header 
            var antiforgeryCookieValue = AntiForgeryTokenExtractor.ExtractCookieValue(httpHeader);
            var antiForgeryVal = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(antiforgeryCookieValue, reqContent);


            var httpPostReq = new HttpRequestMessage(HttpMethod.Post, "/Admin/Add");
            httpPostReq.Headers.Add("Cookie", new CookieHeaderValue(AntiForgeryTokenExtractor.Cookie, antiforgeryCookieValue).ToString());

            string title = "Test Title";
            string description = "Test Description";
            string filePath = "/test_file_path";
            var testImagePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, @"..\..\..\images\TestImage_01.JPG"));

            if (File.Exists(testImagePath))
            {
                var testImage = File.OpenRead(testImagePath);

            }// end if 

            var addProductModel = new Dictionary<string, string>()
            {
                { AntiForgeryTokenExtractor.Field, antiForgeryVal.field },
                { "VehicleModel.Title",  title},
                { "VehicleModel.ProductDescription", description },
                { "VehicleModel.FilePath", filePath },
                { "VehicleModel.ImageName_1", "TestImage.jpg" },
            };

            httpPostReq.Content = new FormUrlEncodedContent(addProductModel);
            var resposne = await client.SendAsync(httpPostReq);
            var responseString = await resposne.Content.ReadAsStringAsync();
            

            // Assert 
            Assert.Equal(HttpStatusCode.Found, resposne.StatusCode);



        }// end AddProduct_HTTP_POST_ValidModel_AsAnAuthenticated_AdminUser_AllowAutoRedirect_IS_False()

        [Fact]
        public async Task AddProduct_HTTP_POST_ValidModel_AsAnAuthenticated_AdminUser_AllowAutoRedirect_IS_True()
        {


            // Arrange 
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddAuthentication("Admin")
                            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Admin", options => { })
                            ;
                });

            })
            .CreateClient(new WebApplicationFactoryClientOptions() { AllowAutoRedirect = true });


            // Act 
            var request = await client.GetAsync("/Admin/Add");


            var httpHeader = request.Headers.ToString();
            var reqContent = await request.Content.ReadAsStringAsync();

            // Getting Cookie Value from HTTP Header 
            var antiforgeryCookieValue = AntiForgeryTokenExtractor.ExtractCookieValue(httpHeader);


            var antiForgeryVal = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(antiforgeryCookieValue, reqContent);


            var httpPostReq = new HttpRequestMessage(HttpMethod.Post, "/Admin/Add");
            httpPostReq.Headers.Add("Cookie", new CookieHeaderValue(AntiForgeryTokenExtractor.Cookie, antiforgeryCookieValue).ToString());

            string title = "Test Title";
            string description = "Lorem ipsum stet voluptua ut et aliquyam ut sea dolore accumsan dolore minim volutpat ex assum sea sed ea vel at molestie sit et et lorem kasd justo kasd ipsum ipsum vel sanctus sed takimata consequat placerat stet accumsan autem justo dolor sit velit lorem dolore velit aliquyam consectetuer sea magna et ex est erat ipsum id nostrud est aliquip aliquam consetetur ut nonumy aliquyam est feugiat labore et lorem sit consetetur est amet ea eos facilisi invidunt eos vero nostrud ut ipsum et accusam clita et amet ea ipsum sanctus et sea lorem aliquyam dolor diam nibh vero consetetur sit possim ut diam kasd invidunt magna duo diam sanctus aliquyam sadipscing invidunt duis sit dolore diam ut praesent duo vulputate aliquyam et accusam ipsum rebum tempor dolor odio diam sit et erat augue justo justo lorem nam kasd ut tation erat gubergren no sadipscing eum veniam voluptua takimata enim ex sed sea nonumy ex accusam magna illum duis duis lorem aliquam ipsum labore kasd magna stet aliquam kasd voluptua justo ipsum vero vel voluptua feugiat et clita duis dolor ea facilisis consetetur sed odio lorem sed et amet duo duo duis et molestie invidunt sed et voluptua ipsum erat sit sed et eu sit labore duo clita clita voluptua sit nostrud eirmod feugiat diam eirmod facilisis sanctus magna takimata eos esse at sadipscing hendrerit dignissim nulla nostrud kasd invidunt erat consetetur stet praesent vel sed augue sit amet et tempor sit clita duo duo no eros voluptua vero et sed voluptua facilisis sed est clita magna takimata nonumy gubergren molestie nibh labore iriure magna takimata sit aliquyam clita et vero magna clita no et sed hendrerit eros elitr et sadipscing est invidunt ad lorem dolor ipsum et clita et sit et sanctus ex illum diam amet ut tempor kasd adipiscing sanctus dolor vero in soluta delenit et et sea et ut option takimata dolores et invidunt dignissim no erat nonummy sed voluptua diam invidunt dolor no sadipscing at kasd labore nibh eirmod magna clita liber sea clita et ut voluptua diam ut dolor dolor sed sed ea invidunt ipsum eirmod at sadipscing no nobis at elit takimata eirmod velit no accusam eum diam sadipscing minim dolore duo et clita at clita ipsum vero sanctus lorem nonumy accumsan tempor te dolor at takimata nam eos no labore aliquyam congue sed voluptua wisi eum sit diam labore vel lorem no sea Description";
            string filePath = "/test_file_path";
            var testImagePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, @"..\..\..\images\TestImage_01.JPG"));

            if (File.Exists(testImagePath))
            {
                var testImage = File.OpenRead(testImagePath);

            }// end if 

            var addProductModel = new Dictionary<string, string>()
            {
                { AntiForgeryTokenExtractor.Field, antiForgeryVal.field },
                { "VehicleModel.Title",  title},
                { "VehicleModel.ProductDescription", description },
                { "VehicleModel.FilePath", filePath },
                { "VehicleModel.ImageName_1", "TestImage.jpg" },
            };

            httpPostReq.Content = new FormUrlEncodedContent(addProductModel);
            var resposne = await client.SendAsync(httpPostReq);
            var responseContent = await resposne.Content.ReadAsStringAsync();


            // Assert 
            Assert.Equal(HttpStatusCode.OK, resposne.StatusCode);
            Assert.Contains(title, responseContent);


        }// end AddProduct_HTTP_POST_ValidModel_AsAnAuthenticated_AdminUser_AllowAutoRedirect_IS_True()

        /// <summary>
        /// TODO: <seealso cref="EditProduct_HTTP_POST_AsAnAuthenticated_AdminUser"/> Haven't Test Yet. 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Fact]
        public async Task EditProduct_HTTP_POST_AsAnAuthenticated_AdminUser()
        {
            // Arrange 
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddAuthentication("Admin")
                            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Admin", options => { })
                            ;
                });

            })
            .CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = true,
            });


            var vehicleList = GetVehicelList().Result; 

          

            // Act 
            var vehicleModel = vehicleList[0]; 
            var request = await client.GetAsync(@$"/Admin/Edit/?id={vehicleModel.Id}");

            var httpHeader = request.Headers.ToString();
            var reqContent = await request.Content.ReadAsStringAsync();

            // Getting Cookie Value from HTTP Header 
            var antiforgeryCookieValue = AntiForgeryTokenExtractor.ExtractCookieValue(httpHeader);


            var antiForgeryVal = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(antiforgeryCookieValue, reqContent);


            var httpPostReq = new HttpRequestMessage(HttpMethod.Post, @$"/Admin/Edit/?id={vehicleModel.Id}");
            httpPostReq.Headers.Add("Cookie", new CookieHeaderValue(AntiForgeryTokenExtractor.Cookie, antiforgeryCookieValue).ToString());


            //// Converting objec to Dictionary
            //// Ref: https://stackoverflow.com/questions/11576886/how-to-convert-object-to-dictionarytkey-tvalue-in-c

            vehicleModel.Title = "Title Edited";

            VehicleViewModel vehicelViewModel = new VehicleViewModel(); 
            vehicelViewModel.VehicleModel = vehicleModel;
            
            
            var serializeData = vehicelViewModel.ToKeyValue();
            // DONOT FORGET to add Antiforgery Value
            serializeData.Add(AntiForgeryTokenExtractor.Field, antiForgeryVal.field); 

            httpPostReq.Content = new FormUrlEncodedContent(serializeData);


            var resposne = await client.SendAsync(httpPostReq);
            var responseString = await resposne.Content.ReadAsStringAsync();


            // Assert 
            Assert.Equal(HttpStatusCode.OK, request.StatusCode);
            Assert.Contains(vehicleModel.Title, responseString);

        }// end EditProduct_HTTP_POST_AsAnAuthenticated_AdminUser()

        /// <summary>
        /// <see cref="DeleteProduct_HTTP_POST_AsAnAuthenticated_AdminUser"/> will delete item by its ID. 
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DeleteProduct_HTTP_POST_AsAnAuthenticated_AdminUser()
        {
            // Arrange 
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddAuthentication("Admin")
                            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Admin", options => { })
                            ;
                });

            })
            .CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = true,
            });


            var vehicleList = GetVehicelList().Result;


            // Act 
            var vehicleModel = vehicleList[0];
            var request = await client.GetAsync(@$"/Admin/Delete/?id={vehicleModel.Id}");

            var httpHeader = request.Headers.ToString();
            var reqContent = await request.Content.ReadAsStringAsync();

            // Getting Cookie Value from HTTP Header 
            var antiforgeryCookieValue = AntiForgeryTokenExtractor.ExtractCookieValue(httpHeader);
            var antiForgeryVal = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(antiforgeryCookieValue, reqContent);


            var httpPostReq = new HttpRequestMessage(HttpMethod.Post, @$"/Admin/Delete/?id={vehicleModel.Id}");
            httpPostReq.Headers.Add("Cookie", new CookieHeaderValue(AntiForgeryTokenExtractor.Cookie, antiforgeryCookieValue).ToString());


            //// Converting objec to Dictionary
            //// Ref: https://stackoverflow.com/questions/11576886/how-to-convert-object-to-dictionarytkey-tvalue-in-c

            var antiforgeryToken = new Dictionary<string, string>()
            {
                { AntiForgeryTokenExtractor.Field, antiForgeryVal.field },
            };

            httpPostReq.Content = new FormUrlEncodedContent(antiforgeryToken);


            var resposne = await client.SendAsync(httpPostReq);
            var responseString = await resposne.Content.ReadAsStringAsync();


            // Assert 
            Assert.Equal(HttpStatusCode.OK, request.StatusCode);
            Assert.DoesNotContain(vehicleModel.Title, responseString);

        }// end EditProduct_HTTP_POST_AsAnAuthenticated_AdminUser()

        #endregion

        #region snippet4 TestAuthHandler
        /// <summary>
        /// <seealso cref="TestAuthHandler"/> allows you to test the Authentication and Authorization. 
        /// The following class <see cref="TestAuthHandler"/> directly inherited from MicroSoft Documentation. 
        /// 
        /// </summary>
        public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
        {
            public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
                ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
                : base(options, logger, encoder, clock)
            {
            }

            protected override Task<AuthenticateResult> HandleAuthenticateAsync()
            {
                var claims = new[] 
                { 
                    new Claim(ClaimTypes.Name, "Test user"), 
                    new Claim(ClaimTypes.Role, "Admin"),    // Admin Role
                };
                var identity = new ClaimsIdentity(claims, "Test");
                var principal = new ClaimsPrincipal(identity);

                var ticket = new AuthenticationTicket(principal, "Test");

                var result = AuthenticateResult.Success(ticket);

                return Task.FromResult(result);
            }
        }// end class TestAuthHandler

        #endregion

        #region Get Vehicle List from DbContext
        private async Task<List<VehicleModel>> GetVehicelList ()
        {
            var vehicleList = new List<VehicleModel>();

            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<ProductDbContext>();
                vehicleList = await dbContext.Vehicles.ToListAsync();
            }// end using

            return vehicleList;

        }// end GetVehicelList ()
        
        #endregion

    }// end class 
}