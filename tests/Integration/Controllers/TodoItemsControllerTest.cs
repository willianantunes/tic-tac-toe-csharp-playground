using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using src.Repository;
using Xunit;

namespace tests.Integration.Controllers
{
    public class TodoItemsControllerTest : IClassFixture<WebApplicationFactory<src.Startup>>
    {
        private HttpClient _httpClient;
        private WebApplicationFactory<src.Startup> _factory;

        public TodoItemsControllerTest(WebApplicationFactory<src.Startup> factory)
        {
            _factory = factory;
            _httpClient = factory.CreateClient();
        }

        [Fact(DisplayName = "Should create new TodoItem and receive 201")]
        public async Task ShouldCreateNewTodoItemAndReceive201()
        {
            var todoItemToBeCreated = new TodoItem();
            todoItemToBeCreated.Name = "Calopsita";
            todoItemToBeCreated.IsComplete = false;

            var contentRequest = new StringContent(JsonConvert.SerializeObject(todoItemToBeCreated), Encoding.UTF8,
                "application/json");
            var response = await _httpClient.PostAsync("/api/TodoItems", contentRequest);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var contentResult = await response.Content.ReadAsStringAsync();
            var createdTodoItem = JsonConvert.DeserializeObject<TodoItem>(contentResult);
            response.Headers.Location.ToString().Should().Be($"http://localhost/api/TodoItems/{createdTodoItem.Id}");
            createdTodoItem.Name.Should().Be(todoItemToBeCreated.Name);
            createdTodoItem.IsComplete.Should().Be(todoItemToBeCreated.IsComplete);
        }

        [Fact(DisplayName = "Should retrieve TodoItems saved previously")]
        public async void ShouldRetrieveTodoItemSavedPreviously()
        {
            using var testPreparationScope = _factory.Services.CreateScope();
            var context = testPreparationScope.ServiceProvider.GetRequiredService<CSharpPlaygroundContext>();
            // CLEAR OLD DATA
            context.TodoItems.RemoveRange(context.TodoItems);
            await context.SaveChangesAsync();
            // PREPARE TEST
            var createdTodoItem = new TodoItem {Name = "Jafar", IsComplete = true};
            context.TodoItems.Add(createdTodoItem);
            await context.SaveChangesAsync();

            // EVALUATION
            var response = await _httpClient.GetAsync("/api/TodoItems/");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var contentResult = await response.Content.ReadAsStringAsync();
            var todoItems = JsonConvert.DeserializeObject<List<TodoItem>>(contentResult);

            todoItems.Should().HaveCount(1);
            var todoItem = todoItems.First();
            todoItem.Name.Should().Be(createdTodoItem.Name);
            todoItem.IsComplete.Should().Be(createdTodoItem.IsComplete);
            todoItem.Id.Should().Be(createdTodoItem.Id);
        }

        [Fact(DisplayName = "Should update TodoItem")]
        public async void ShouldUpdateTodoItemSavedPreviously()
        {
            using var testPreparationScope = _factory.Services.CreateScope();
            var context = testPreparationScope.ServiceProvider.GetRequiredService<CSharpPlaygroundContext>();
            var createdTodoItem = new TodoItem {Name = "Jafar", IsComplete = true};
            context.TodoItems.Add(createdTodoItem);
            await context.SaveChangesAsync();

            var newValuesForCreatedTodoItem = new
            {
                Id = createdTodoItem.Id,
                Name = "Jafar and Iago",
                IsComplete = false
            };

            var serializeObject = JsonConvert.SerializeObject(newValuesForCreatedTodoItem);
            var contentRequest = new StringContent(serializeObject, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/api/TodoItems/{createdTodoItem.Id}", contentRequest);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // TODO: Create a new context or refresh this one to get updatedTodoItem as expected
            // var updatedTodoItem = await _context.TodoItems.FindAsync(createdTodoItem.Id);
            // updatedTodoItem.Name.Should().Be(todoItemToBeUpdate.Name);
            // updatedTodoItem.IsComplete.Should().Be(todoItemToBeUpdate.IsComplete);
        }

        [Fact(DisplayName = "Should delete TodoItem")]
        public async void ShouldDeleteTodoItem()
        {
            using var testPreparationScope = _factory.Services.CreateScope();
            var context = testPreparationScope.ServiceProvider.GetRequiredService<CSharpPlaygroundContext>();
            var createdTodoItem = new TodoItem {Name = "Aladdin", IsComplete = true};
            context.TodoItems.Add(createdTodoItem);
            await context.SaveChangesAsync();

            var response = await _httpClient.DeleteAsync($"/api/TodoItems/{createdTodoItem.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            // TODO: Check if the database has zero entries
        }
    }
}
