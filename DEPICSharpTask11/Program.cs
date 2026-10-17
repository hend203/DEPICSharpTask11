using static DEPICSharpTask11.ListGenerator;

namespace DEPICSharpTask11
{
    internal class Program
    {
        // Prints a small title before the result of every question
        private static void Title(string text)
        {
            Console.WriteLine();
            Console.WriteLine($"=================== {text} ===================");
        }

        // Prints every item of a sequence in its own line
        private static void Print<T>(IEnumerable<T> sequence)
        {
            foreach (T item in sequence)
                Console.WriteLine(item);
        }

        // Prints every group: its key, then its elements
        private static void PrintGroups<TKey, TElement>(IEnumerable<IGrouping<TKey, TElement>> groups)
        {
            foreach (IGrouping<TKey, TElement> grp in groups)
            {
                Console.WriteLine(grp.Key);

                foreach (TElement item in grp)
                    Console.WriteLine($"   {item}");
            }
        }

        // Last three characters of a name (the whole name if it is shorter than 3 characters)
        private static string LastThree(string name)
            => name.Length >= 3 ? name[^3..] : name;

        static void Main(string[] args)
        {
            #region Input Data (shared between many questions)
            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

            // dictionary_english.txt is read into an array of strings FIRST
            string[] dictionaryWords = File.ReadAllLines(Path.Combine(AppContext.BaseDirectory, "dictionary_english.txt"));
            #endregion

            #region LINQ - Element Operators (Immediate Execution, No Query Syntax)

            #region Q1: Get first Product out of Stock
            Title("Element Q1: First product out of stock");
            var firstOutOfStock = ProductsList.First(p => p.UnitsInStock == 0);

            Console.WriteLine(firstOutOfStock);
            #endregion

            #region Q2: Return the first product whose Price > 1000, unless there is no match => null
            Title("Element Q2: First product whose price > 1000 (or null)");
            // First() throws InvalidOperationException when nothing matches, FirstOrDefault() returns null
            var expensiveProduct = ProductsList.FirstOrDefault(p => p.UnitPrice > 1000);

            Console.WriteLine(expensiveProduct is null ? "null" : expensiveProduct.ToString());
            #endregion

            #region Q3: Retrieve the second number greater than 5
            Title("Element Q3: Second number greater than 5");
            // Numbers greater than 5 (in order): 9, 8, 6, 7 => the second one is 8
            int secondGreaterThanFive = numbers.Where(n => n > 5).ElementAt(1);

            Console.WriteLine(secondGreaterThanFive);
            #endregion

            #endregion

            #region LINQ - Aggregate Operators (Immediate Execution)

            #region Q1: Use Count to get the number of odd numbers in the array
            Title("Aggregate Q1: Count of odd numbers");
            int oddNumbersCount = numbers.Count(n => n % 2 != 0);

            Console.WriteLine(oddNumbersCount); // 5
            #endregion

            #region Q2: Return a list of customers and how many orders each has
            Title("Aggregate Q2: Customers and their orders count");
            // Fluent Syntax
            var customersOrdersCount = CustomersList.Select(c => new { c.CustomerID, c.CustomerName, OrdersCount = c.Orders.Count() });
            // Query Syntax
            //var customersOrdersCount = from c in CustomersList
            //                           select new { c.CustomerID, c.CustomerName, OrdersCount = c.Orders.Count() };

            Print(customersOrdersCount);
            #endregion

            #region Q3: Return a list of categories and how many products each has
            Title("Aggregate Q3: Categories and their products count");
            // Fluent Syntax
            var categoriesProductsCount = ProductsList.GroupBy(p => p.Category)
                                                      .Select(g => new { Category = g.Key, ProductsCount = g.Count() });
            // Query Syntax
            //var categoriesProductsCount = from p in ProductsList
            //                              group p by p.Category into g
            //                              select new { Category = g.Key, ProductsCount = g.Count() };

            Print(categoriesProductsCount);
            #endregion

            #region Q4: Get the total of the numbers in an array
            Title("Aggregate Q4: Total of the numbers");
            int totalOfNumbers = numbers.Sum();

            Console.WriteLine(totalOfNumbers); // 45
            #endregion

            #region Q5: Get the total number of characters of all words in dictionary_english.txt
            Title("Aggregate Q5: Total number of characters");
            int totalCharacters = dictionaryWords.Sum(w => w.Length);

            Console.WriteLine(totalCharacters);
            #endregion

            #region Q6: Get the length of the shortest word in dictionary_english.txt
            Title("Aggregate Q6: Length of the shortest word");
            int shortestWordLength = dictionaryWords.Min(w => w.Length);

            Console.WriteLine(shortestWordLength);
            #endregion

            #region Q7: Get the length of the longest word in dictionary_english.txt
            Title("Aggregate Q7: Length of the longest word");
            int longestWordLength = dictionaryWords.Max(w => w.Length);

            Console.WriteLine(longestWordLength);
            #endregion

            #region Q8: Get the average length of the words in dictionary_english.txt
            Title("Aggregate Q8: Average length of the words");
            double averageWordLength = dictionaryWords.Average(w => w.Length);

            Console.WriteLine(averageWordLength);
            #endregion

            #region Q9: Get the total units in stock for each product category
            Title("Aggregate Q9: Total units in stock for each category");
            // Fluent Syntax
            var unitsInStockPerCategory = ProductsList.GroupBy(p => p.Category)
                                                      .Select(g => new { Category = g.Key, TotalUnitsInStock = g.Sum(p => p.UnitsInStock) });
            // Query Syntax
            //var unitsInStockPerCategory = from p in ProductsList
            //                              group p by p.Category into g
            //                              select new { Category = g.Key, TotalUnitsInStock = g.Sum(x => x.UnitsInStock) };

            Print(unitsInStockPerCategory);
            #endregion

            #region Q10: Get the cheapest price among each category's products
            Title("Aggregate Q10: Cheapest price in each category");
            // Fluent Syntax
            var cheapestPricePerCategory = ProductsList.GroupBy(p => p.Category)
                                                       .Select(g => new { Category = g.Key, CheapestPrice = g.Min(p => p.UnitPrice) });
            // Query Syntax
            //var cheapestPricePerCategory = from p in ProductsList
            //                               group p by p.Category into g
            //                               select new { Category = g.Key, CheapestPrice = g.Min(x => x.UnitPrice) };

            Print(cheapestPricePerCategory);
            #endregion

            #region Q11: Get the products with the cheapest price in each category (Use Let)
            Title("Aggregate Q11: Products with the cheapest price in each category");
            // Query Syntax (let stores the cheapest price of the group once, then it is reused in the select)
            var cheapestProducts = from p in ProductsList
                                   group p by p.Category into g
                                   let minPrice = g.Min(x => x.UnitPrice)
                                   select new { Category = g.Key, Products = g.Where(x => x.UnitPrice == minPrice) };
            // Fluent Syntax (there is no "let" keyword in Fluent, a Select creates the extra variable)
            //var cheapestProducts = ProductsList.GroupBy(p => p.Category)
            //                                   .Select(g => new { g, MinPrice = g.Min(x => x.UnitPrice) })
            //                                   .Select(x => new { Category = x.g.Key, Products = x.g.Where(p => p.UnitPrice == x.MinPrice) });

            foreach (var item in cheapestProducts)
            {
                Console.WriteLine(item.Category);

                foreach (var product in item.Products)
                    Console.WriteLine($"   {product}");
            }
            #endregion

            #region Q12: Get the most expensive price among each category's products
            Title("Aggregate Q12: Most expensive price in each category");
            // Fluent Syntax
            var mostExpensivePricePerCategory = ProductsList.GroupBy(p => p.Category)
                                                            .Select(g => new { Category = g.Key, MostExpensivePrice = g.Max(p => p.UnitPrice) });
            // Query Syntax
            //var mostExpensivePricePerCategory = from p in ProductsList
            //                                    group p by p.Category into g
            //                                    select new { Category = g.Key, MostExpensivePrice = g.Max(x => x.UnitPrice) };

            Print(mostExpensivePricePerCategory);
            #endregion

            #region Q13: Get the products with the most expensive price in each category (Use Let)
            Title("Aggregate Q13: Products with the most expensive price in each category");
            // Query Syntax
            var mostExpensiveProducts = from p in ProductsList
                                        group p by p.Category into g
                                        let maxPrice = g.Max(x => x.UnitPrice)
                                        select new { Category = g.Key, Products = g.Where(x => x.UnitPrice == maxPrice) };

            foreach (var item in mostExpensiveProducts)
            {
                Console.WriteLine(item.Category);

                foreach (var product in item.Products)
                    Console.WriteLine($"   {product}");
            }
            #endregion

            #region Q14: Get the average price of each category's products
            Title("Aggregate Q14: Average price of each category");
            // Fluent Syntax
            var averagePricePerCategory = ProductsList.GroupBy(p => p.Category)
                                                      .Select(g => new { Category = g.Key, AveragePrice = g.Average(p => p.UnitPrice) });
            // Query Syntax
            //var averagePricePerCategory = from p in ProductsList
            //                              group p by p.Category into g
            //                              select new { Category = g.Key, AveragePrice = g.Average(x => x.UnitPrice) };

            Print(averagePricePerCategory);
            #endregion

            #endregion

            #region LINQ - Set Operators (Union Family)

            #region Q1: Find the unique Category names from Product List
            Title("Set Q1: Unique category names");
            var uniqueCategories = ProductsList.Select(p => p.Category)
                                               .Distinct();

            Print(uniqueCategories);
            #endregion

            // First letters used by Q2, Q3 and Q4
            var productFirstLetters = ProductsList.Select(p => p.ProductName[0]);
            var customerFirstLetters = CustomersList.Select(c => c.CustomerName[0]);

            #region Q2: Sequence containing the unique first letter from both product and customer names
            Title("Set Q2: Unique first letters of products and customers (Union)");
            var uniqueFirstLetters = productFirstLetters.Union(customerFirstLetters);

            Print(uniqueFirstLetters);
            #endregion

            #region Q3: Sequence containing the common first letter from both product and customer names
            Title("Set Q3: Common first letters (Intersect)");
            var commonFirstLetters = productFirstLetters.Intersect(customerFirstLetters);

            Print(commonFirstLetters);
            #endregion

            #region Q4: First letters of product names that are not also first letters of customer names
            Title("Set Q4: Product first letters that are not customer first letters (Except)");
            var productOnlyFirstLetters = productFirstLetters.Except(customerFirstLetters);

            Print(productOnlyFirstLetters);
            #endregion

            #region Q5: Last three characters of each name of all customers and products, including duplicates
            Title("Set Q5: Last three characters of all names (Concat keeps the duplicates)");
            var lastThreeCharacters = CustomersList.Select(c => LastThree(c.CustomerName))
                                                   .Concat(ProductsList.Select(p => LastThree(p.ProductName)));

            Print(lastThreeCharacters);
            #endregion

            #endregion

            #region LINQ - Partitioning Operators

            #region Q1: Get the first 3 orders from customers in Washington
            Title("Partitioning Q1: First 3 orders from customers in Washington");
            // Washington = Region "WA"
            var firstThreeWashingtonOrders = CustomersList.Where(c => c.Region == "WA")
                                                          .SelectMany(c => c.Orders)
                                                          .Take(3);

            Print(firstThreeWashingtonOrders);
            #endregion

            #region Q2: Get all but the first 2 orders from customers in Washington
            Title("Partitioning Q2: All but the first 2 orders from customers in Washington");
            var washingtonOrdersWithoutFirstTwo = CustomersList.Where(c => c.Region == "WA")
                                                               .SelectMany(c => c.Orders)
                                                               .Skip(2);

            Print(washingtonOrdersWithoutFirstTwo);
            #endregion

            #region Q3: Elements from the beginning until a number is hit that is less than its position
            Title("Partitioning Q3: TakeWhile the number is not less than its position");
            // Continue while n >= position. 5 >= 0, 4 >= 1, but 1 < 2 => stops => 5, 4
            var untilLessThanPosition = numbers.TakeWhile((n, i) => n >= i);

            Print(untilLessThanPosition);
            #endregion

            #region Q4: Elements starting from the first element divisible by 3
            Title("Partitioning Q4: SkipWhile the number is not divisible by 3");
            // 5, 4, 1 are skipped, the first element divisible by 3 is 3 => 3, 9, 8, 6, 7, 2, 0
            var fromFirstDivisibleByThree = numbers.SkipWhile(n => n % 3 != 0);

            Print(fromFirstDivisibleByThree);
            #endregion

            #region Q5: Elements starting from the first element less than its position
            Title("Partitioning Q5: SkipWhile the number is not less than its position");
            // 5 >= 0, 4 >= 1 are skipped, the first element less than its position is 1 (position 2) => 1, 3, 9, 8, 6, 7, 2, 0
            var fromFirstLessThanPosition = numbers.SkipWhile((n, i) => n >= i);

            Print(fromFirstLessThanPosition);
            #endregion

            #endregion

            #region LINQ - Quantifiers

            #region Q1: Determine if any of the words in dictionary_english.txt contain the substring 'ei'
            Title("Quantifier Q1: Any word contains 'ei'?");
            bool anyWordContainsEi = dictionaryWords.Any(w => w.Contains("ei"));

            Console.WriteLine(anyWordContainsEi);
            #endregion

            #region Q2: Grouped list of products only for categories that have at least one product out of stock
            Title("Quantifier Q2: Categories that have at least one product out of stock");
            // Query Syntax
            var categoriesWithOutOfStock = from p in ProductsList
                                           group p by p.Category into g
                                           where g.Any(x => x.UnitsInStock == 0)
                                           select g;
            // Fluent Syntax
            //var categoriesWithOutOfStock = ProductsList.GroupBy(p => p.Category)
            //                                           .Where(g => g.Any(p => p.UnitsInStock == 0));

            PrintGroups(categoriesWithOutOfStock);
            #endregion

            #region Q3: Grouped list of products only for categories that have all of their products in stock
            Title("Quantifier Q3: Categories that have all of their products in stock");
            // Query Syntax
            var categoriesAllInStock = from p in ProductsList
                                       group p by p.Category into g
                                       where g.All(x => x.UnitsInStock > 0)
                                       select g;
            // Fluent Syntax
            //var categoriesAllInStock = ProductsList.GroupBy(p => p.Category)
            //                                       .Where(g => g.All(p => p.UnitsInStock > 0));

            PrintGroups(categoriesAllInStock);
            #endregion

            #endregion

            #region LINQ - Grouping Operators

            #region Q1: Partition a list of numbers by their remainder when divided by 5
            Title("Grouping Q1: Numbers grouped by their remainder when divided by 5");
            List<int> remainderNumbers = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            // Query Syntax
            var numbersByRemainder = from n in remainderNumbers
                                     group n by n % 5;
            // Fluent Syntax
            //var numbersByRemainder = remainderNumbers.GroupBy(n => n % 5);

            foreach (var remainderGroup in numbersByRemainder)
            {
                Console.WriteLine($"Numbers with a remainder of {remainderGroup.Key} when divided by 5:");

                foreach (int number in remainderGroup)
                    Console.WriteLine(number);
            }
            #endregion

            #region Q2: Partition a list of words by their first letter (input: dictionary_english.txt)
            Title("Grouping Q2: Dictionary words grouped by first letter");
            // Query Syntax
            var wordsByFirstLetter = from w in dictionaryWords
                                     group w by w[0];
            // Fluent Syntax
            //var wordsByFirstLetter = dictionaryWords.GroupBy(w => w[0]);

            foreach (var letterGroup in wordsByFirstLetter)
            {
                Console.WriteLine($"Words that start with '{letterGroup.Key}': {letterGroup.Count()} words");

                // The dictionary has 370,000+ words, so only the first 5 words of every letter are printed.
                // Remove .Take(5) to print all of them.
                foreach (string word in letterGroup.Take(5))
                    Console.WriteLine($"   {word}");
            }
            #endregion

            #region Q3: Group With a custom comparer: words that consist of the same characters together
            Title("Grouping Q3: Words that consist of the same characters (custom comparer)");
            string[] anagramWords = { "from", "salt", "earn", "last", "near", "form" };
            // Fluent Syntax (the custom comparer is passed to GroupBy)
            var anagramGroups = anagramWords.GroupBy(w => w, new AnagramComparer());
            // Query Syntax => No way to pass a custom comparer in the Query Syntax

            foreach (var anagramGroup in anagramGroups)
            {
                foreach (string word in anagramGroup)
                    Console.WriteLine(word);

                Console.WriteLine("....");
            }
            #endregion

            #endregion
        }
    }
}
