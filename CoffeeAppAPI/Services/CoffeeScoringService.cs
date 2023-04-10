using System;
using System.Collections.Generic;
using System.Linq;
using CoffeeAppAPI.Models;

namespace CoffeeAppAPI.Services
{

    public interface ICoffeeScoringService
    {
        Dictionary<(Guid, Guid), double> Matrix { get; }
        double CalculatePreferenceScore(UserPreferences userPreferences, Coffee coffee);
        List<Guid> GenerateRecommendations(Models.User user, List<Coffee> allCoffees, int numberOfRecommendations);
        void UpdateCoffeeSimilarityMatrix(List<Coffee> coffees);
    }

    public class CoffeeScoringService : ICoffeeScoringService
    {
        public Dictionary<(Guid, Guid), double> Matrix { get; private set; }
        private readonly IServiceProvider _serviceProvider;

        public CoffeeScoringService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            using (var scope = _serviceProvider.CreateScope())
            {
                var coffeeService = scope.ServiceProvider.GetRequiredService<ICoffeeService>();
                var coffees = coffeeService.GetAllAsync().Result;
                
                Matrix = ComputeCoffeeSimilarityMatrix(coffees.ToList());
            }
        }

        public List<Guid> GenerateRecommendations(Models.User user, List<Coffee> allCoffees, int numberOfRecommendations)
        {
            var recommendations = new List<Guid>();

            foreach (Coffee c in allCoffees)
            {
                Console.WriteLine($"Coffee {c.id} has name {c.CoffeeName}");
            }

            // Compute scores for each coffee based on user preferences and the similarity matrix.
            var coffeeScores = new Dictionary<Guid, double>();
            foreach (var coffee in allCoffees)
            {
                double preferenceScore = CalculatePreferenceScore(user.Preferences, coffee);
                double similarityScoreSum = 0;

                foreach (var otherCoffee in allCoffees)
                {
                    if (coffee.id != otherCoffee.id)
                    {
                        if (!Matrix.ContainsKey((coffee.id, otherCoffee.id)))
                        {
                            Console.WriteLine($"Matrix missing key: ({coffee.id}, {otherCoffee.id})");
                        }
                        similarityScoreSum += Matrix[(coffee.id, otherCoffee.id)];
                    }
                }

                double similarityScoreAvg = similarityScoreSum / (allCoffees.Count - 1);
                double combinedScore = (preferenceScore + similarityScoreAvg) / 2;
                Console.WriteLine($"Coffee {coffee.id} has preference score {preferenceScore}, similarity score {similarityScoreAvg}, and combined score {combinedScore}");
                coffeeScores[coffee.id] = combinedScore;
            }

            // Sort the coffees based on their scores.
            var sortedCoffees = coffeeScores.OrderByDescending(c => c.Value).Select(c => c.Key).ToList();

            // Recommend the top N coffees.
            for (int i = 0; i < numberOfRecommendations && i < sortedCoffees.Count; i++)
            {
                recommendations.Add(sortedCoffees[i]);
                Console.WriteLine($"Recommended coffee {i + 1}: {sortedCoffees[i]} with score {coffeeScores[sortedCoffees[i]]}");
            }

            return recommendations;
        }


        public Dictionary<(Guid, Guid), double> ComputeCoffeeSimilarityMatrix(List<Coffee> coffees)
        {
            var similarityMatrix = new Dictionary<(Guid, Guid), double>();

            foreach (var coffee1 in coffees)
            {
                foreach (var coffee2 in coffees)
                {
                    if (!similarityMatrix.ContainsKey((coffee1.id, coffee2.id)) && !similarityMatrix.ContainsKey((coffee2.id, coffee1.id)))
                    {
                        // Console.WriteLine($"Computing similarity for coffees {coffee1.id} and {coffee2.id}");
                        double similarityScore = coffee1.id == coffee2.id ? 1 : CalculateSimilarity(coffee1, coffee2);
                        similarityMatrix[(coffee1.id, coffee2.id)] = similarityScore;
                        similarityMatrix[(coffee2.id, coffee1.id)] = similarityScore;
                    }
                }
            }
            return similarityMatrix;
        }


        public double CalculateSimilarity(Coffee coffee1, Coffee coffee2)
        {
            double coffeeTypeSimilarity = coffee1.CoffeeType == coffee2.CoffeeType ? 1 : 0;
            double roastLevelSimilarity = coffee1.RoastLevel == coffee2.RoastLevel ? 1 : 0;

            int flavorNoteIntersection = 0;
            int flavorNoteUnion = 0;

            if (coffee1.FlavorNotes != null && coffee2.FlavorNotes != null)
            {
                flavorNoteIntersection = coffee1.FlavorNotes.Intersect(coffee2.FlavorNotes).Count();
                flavorNoteUnion = coffee1.FlavorNotes.Union(coffee2.FlavorNotes).Count();
            }

            double flavorNoteSimilarity = flavorNoteUnion == 0 ? 0 : (double)flavorNoteIntersection / flavorNoteUnion;
            double originSimilarity = coffee1.Origin == coffee2.Origin ? 1 : 0;

            // Combine the individual similarity scores using an averaging approach.
            // You can also use a weighted sum approach if you want to give more importance to certain attributes.
            double overallSimilarity = (coffeeTypeSimilarity + roastLevelSimilarity + flavorNoteSimilarity + originSimilarity) / 4;
            // Console.WriteLine($"Similarity between {coffee1.id} and {coffee2.id}: {overallSimilarity}");
            return overallSimilarity;
        }

        public double CalculatePreferenceScore(UserPreferences userPreferences, Coffee coffee)
        {
            double coffeeTypeScore = userPreferences.CoffeeTypePreferences.Contains(coffee.CoffeeType) ? 1 : 0;
            double roastLevelScore = userPreferences.RoastLevelPreferences.Contains(coffee.RoastLevel) ? 1 : 0;

            int flavorNoteMatches = coffee.FlavorNotes.Intersect(userPreferences.FlavorNotePreferences).Count();
            double flavorNoteScore = (double)flavorNoteMatches / userPreferences.FlavorNotePreferences.Count;

            double originScore = userPreferences.OriginPreferences.Contains(coffee.Origin) ? 1 : 0;

            // Combine the individual preference scores using an averaging approach.
            // You can also use a weighted sum approach if you want to give more importance to certain attributes.
            double overallPreferenceScore = (coffeeTypeScore + roastLevelScore + flavorNoteScore + originScore) / 4;

            return overallPreferenceScore;
        }

        public void UpdateCoffeeSimilarityMatrix(List<Coffee> coffees)
        {
            Matrix = ComputeCoffeeSimilarityMatrix(coffees);
        }


    }
}