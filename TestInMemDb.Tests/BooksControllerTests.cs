using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using TestInMemDb.Data;
using Xunit;

namespace TestInMemDb.Tests
{
    public class BooksControllerTests : IClassFixture<InMemDbWebApplicationFactory>
    {
        HttpClient _httpClient;
        private ServiceProvider _serviceProvider;

        public BooksControllerTests(InMemDbWebApplicationFactory factory)
        {
            _httpClient = factory.CreateClient();
            _serviceProvider = factory.ServiceProvider;
        }


        [Fact]
        public async void TestGetBooks()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TestDbContext>();
                var book1 = new Book { Name = "Book1" };
                var book2 = new Book { Name = "Book2" };
                context.Books.AddRange(book1, book2);
                context.SaveChanges();

                var resp = await _httpClient.GetAsync("/api/books");
                resp.EnsureSuccessStatusCode();

                var stringResponse = await resp.Content.ReadAsStringAsync();
                var books = JsonConvert.DeserializeObject<IEnumerable<Book>>(stringResponse);
                Assert.Contains(books, p => p.Name == "Book1");
                Assert.Contains(books, p => p.Name == "Book1");
            }

        }

        [Fact]
        public async void TestPostBook()
        {
            using (var scope = _serviceProvider.CreateScope()){
                var context = scope.ServiceProvider.GetRequiredService<TestDbContext>();
                var book1 = new Book { Name = "Book1" };
                var book2 = new Book { Name = "Book2" };
                context.Books.AddRange(book1, book2);
                context.SaveChanges();

                var resp = await _httpClient.PostAsJsonAsync("/api/books", "Book3");

                resp.EnsureSuccessStatusCode();
                var stringResponse = await resp.Content.ReadAsStringAsync();
                var book3Id = JsonConvert.DeserializeObject<int>(stringResponse);
                var book3 = context.Books.Find(book3Id);
                Assert.NotNull(book3);
                Assert.Equal("Book3", book3.Name);
            }
        }

        [Fact]
        public async void TestPutBook()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TestDbContext>();
                var book1 = new Book { Name = "Book1" };
                var book2 = new Book { Name = "Book2" };
                context.Books.AddRange(book1, book2);
                context.SaveChanges();

                var updateResp = await _httpClient.PutAsJsonAsync($"/api/books/{book1.Id}", "Book1Update");
                updateResp.EnsureSuccessStatusCode();

                var getResp = await _httpClient.GetAsync($"/api/books/{book1.Id}");
                getResp.EnsureSuccessStatusCode();
                var stringResponse = await getResp.Content.ReadAsStringAsync();
                book1 = JsonConvert.DeserializeObject<Book>(stringResponse);
                Assert.NotNull(book1);
                //this works
                Assert.Equal("Book1Update", book1.Name);

                book1 = context.Books.Find(book1.Id);
                context.Entry(book1).Reload(); // refresh to make sure we can get the book1 
                
                Assert.NotNull(book1);
                
                Assert.Equal("Book1Update", book1.Name);
            }
        }

        [Fact]
        public async void TestPut2Book()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                Book book1;
                var context = scope.ServiceProvider.GetRequiredService<TestDbContext>();
                book1 = new Book { Name = "Book1" };
                context.Books.AddRange(book1);
                context.SaveChanges();


                var updateResp = await _httpClient.PutAsJsonAsync($"/api/books/{book1.Id}", "Book1Update");
                updateResp.EnsureSuccessStatusCode();

                using(var scope2= scope.ServiceProvider.CreateScope()){
                    var context2 = scope2.ServiceProvider.GetRequiredService<TestDbContext>();
                    
                    book1 = context2.Books.Find(book1.Id);
                    Assert.NotNull(book1);
                    Assert.Equal("Book1Update", book1.Name);
                }
            }
        }
    }
}
