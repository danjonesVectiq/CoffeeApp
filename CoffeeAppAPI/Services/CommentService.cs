using CoffeeAppAPI.Models;
using CoffeeAppAPI.Repositories;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeAppAPI.Services
{
    public class CommentService
    {
        private readonly ICosmosDbRepository _cosmosDbRepository;

        public CommentService(ICosmosDbRepository cosmosDbRepository)
        {
            _cosmosDbRepository = cosmosDbRepository;
        }

        public async Task<Container> GetCommentsContainerAsync()
        {
            return await _cosmosDbRepository.GetOrCreateContainerAsync("Interaction", "/id");
        }

        public async Task<IEnumerable<Comment>> GetAllCommentsAsync()
        {
            var commentsContainer = await GetCommentsContainerAsync();
            return await _cosmosDbRepository.GetAllItemsAsync<Comment>(commentsContainer, "Comment");
        }

        public async Task<Comment> GetCommentAsync(Guid id)
        {
            var commentsContainer = await GetCommentsContainerAsync();
            return await _cosmosDbRepository.GetItemAsync<Comment>(commentsContainer, id.ToString());
        }

        public async Task CreateCommentAsync(Comment comment)
        {
            var commentsContainer = await GetCommentsContainerAsync();
            await _cosmosDbRepository.AddItemAsync(commentsContainer, comment);
        }

        public async Task UpdateCommentAsync(Comment comment)
        {
            var commentsContainer = await GetCommentsContainerAsync();
            await _cosmosDbRepository.UpdateItemAsync(commentsContainer, comment.id.ToString(), comment);
        }

        public async Task DeleteCommentAsync(Guid id)
        {
            var commentsContainer = await GetCommentsContainerAsync();
            await _cosmosDbRepository.DeleteItemAsync<Comment>(commentsContainer, id.ToString());
        }
    }
}
