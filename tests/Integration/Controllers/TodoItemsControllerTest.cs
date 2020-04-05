using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using src.Repository;
using tests.Resources;
using Xunit;

namespace tests.Integration.Controllers
{
    public class TodoItemsControllerTest : IClassFixture<DatabaseAndTestServerFixture>
    {
        private readonly DatabaseAndTestServerFixture _testServerFixture;
        private HttpClient _httpClient;
        private CSharpPlaygroundContext _context;

        public TodoItemsControllerTest(DatabaseAndTestServerFixture testServerFixture)
        {
            _httpClient = testServerFixture.HttpClient;
            _context = testServerFixture.CSharpPlaygroundContext;
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
            var createdTodoItem = new TodoItem {Name = "Jafar", IsComplete = true};
            _context.TodoItems.Add(createdTodoItem);
            await _context.SaveChangesAsync();

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
            var createdTodoItem = new TodoItem {Name = "Jafar", IsComplete = true};
            _context.TodoItems.Add(createdTodoItem);
            await _context.SaveChangesAsync();

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
            var createdTodoItem = new TodoItem {Name = "Aladdin", IsComplete = true};
            _context.TodoItems.Add(createdTodoItem);
            await _context.SaveChangesAsync();

            var response = await _httpClient.DeleteAsync($"/api/TodoItems/{createdTodoItem.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            // TODO: Check if the database has zero entries
        }
    }
}