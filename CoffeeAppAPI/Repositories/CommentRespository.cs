using CoffeeAppAPI.Models;
using CoffeeAppAPI.Services;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Repositories
{
    public class CommentRepository
    {
        private readonly ICosmosDbService _cosmosDbService;

        public CommentRepository(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        public async Task<Container> GetCommentsContainerAsync()
        {
            return await _cosmosDbService.GetOrCreateContainerAsync("Comments", "/id");
        }

        public async Task<IEnumerable<Comment>> GetAllCommentsAsync()
        {
            var commentsContainer = await GetCommentsContainerAsync();
            return await _cosmosDbService.GetAllItemsAsync<Comment>(commentsContainer);
        }

        public async Task<Comment> GetCommentAsync(Guid id)
        {
            var commentsContainer = await GetCommentsContainerAsync();
            return await _cosmosDbService.GetItemAsync<Comment>(commentsContainer, id.ToString());
        }

        public async Task CreateCommentAsync(Comment comment)
        {
            var commentsContainer = await GetCommentsContainerAsync();
            await _cosmosDbService.AddItemAsync(commentsContainer, comment);
        }

        public async Task UpdateCommentAsync(Comment comment)
        {
            var commentsContainer = await GetCommentsContainerAsync();
            await _cosmosDbService.UpdateItemAsync(commentsContainer, comment.id.ToString(), comment);
        }

        public async Task DeleteCommentAsync(Guid id)
        {
            var commentsContainer = await GetCommentsContainerAsync();
            await _cosmosDbService.DeleteItemAsync<Comment>(commentsContainer, id.ToString());
        }
    }
}
