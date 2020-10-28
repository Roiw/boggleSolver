# A Fast Boggle Solver

In order to solve the Boggle board, we must search for all possible words we can form starting at each cell of the board. From a cell we have 8 possible directions to go, each direction will provide us with a character.
Every time we add a character to the word we are building, we must check if this word exists in the dictionary provided, if so we found a word. We will check every possible path starting from each cell of the board. The search algorithm we will use for this is a depth first search (DFS).

## Optimizing

In order to optimize our solution we need a data structure that could guide our search, enabling our algorithm to prune our search tree. We will use a trie for this. 
If we load the provided dictionary on a trie, we can traverse both trie and table at the same time. For each of the 8 possible directions we can pick only the ones that lead to a character that exists on our current trie node. In that way our algorithm avoids picking characters that won't lead to a word.
Another great optimization is using a StringBuilder instead of string to keep track of the word. In C# strings are immutable so for every character that we add, a new string is created O(N) complexity. Using a StringBuilder appending characters is an O(1) operation.

## Highlights

- Considering 'Q' as 'Qu' like in a Boggle board.
- Loads the dictionary on a Trie structure.
- For each character on the Boggle board start a DFS
- Traverse the Trie together with the DFS, using the Trie to prune invalid paths.
- Use a StringBuilder for fast character concatenation.
- Complexity is O(N * L) where N is the number of cells on the board and L is the length of the biggest word on the dictionary.  
