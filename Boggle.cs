/*
 * Copyright (c) 2020 Lucas Martins de Souza (https://github.com/Roiw).
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
 * associated documentation files (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
 
using System.Collections.Generic;
using System.Text;

namespace boggleSolver
{
    public class Boggle
    {

        /*
            Strategy: 
                - Use a trie to map all words that are valid.
                - Perform DFS from each cell of the board, (use an equation to convert [x,y] to index)
                - Use a HashSet to make sure we are not going over the same letter twice (save them by index).
            
            Runtime complexity should be O(N * L) where N is the amount of characters on the board and 
            L is the length of the biggest word.

            Space complexity should be O(L) where L is the length of the biggest word.
        */ 

        /// <summary>
        /// A list of position offsets allowed on the board. 
        /// In order: Right, Left, Down, Up, Down-Right, Up-Right, Down-Left, Up-Left.
        /// </summary>
        /// <typeparam name="(int">X index offset</typeparam>
        /// <typeparam name="int)">Y index offset</typeparam>
        private readonly List<(int, int)> positions = new List<(int, int)>() { (0, 1), (0, -1), (1, 0),
         (-1, 0), (1, 1), (-1, 1), (1, -1), (-1, -1)};

        private HashSet<string> wordsFound; // All words found.
        private HashSet<(int,int)> positionsChecked; // Positions that we checked.
        private Trie legalWords; // The dictionary on a Trie structure.
        private int boardWidth, boardHeight; // The board width and height.
        private string boardLetters; // All board letters.

        /// <summary>
        /// Prior to solving a board, configure legal words
        /// </summary>
        /// <param name="allWords">The legal words, alphabetically-sorted.</param>
        public void SetLegalWords(IEnumerable<string> allWords)
        {
            this.legalWords = new Trie('^');

            // your code here
            foreach (string word in allWords)
            {
                this.legalWords.AddWord(word, 0);
            }     
        }

        /// <summary>
        /// Find all words on the specified board.
        /// </summary>
        /// <param name="boardWidth">Width of the board.</param>
        /// <param name="boardHeight">Height of the board.</param>
        /// <param name="boardLetters">Board width*height characters in row major order.</param>
        /// <returns></returns>
        public IEnumerable<string> SolveBoard(int boardWidth, int boardHeight, string boardLetters)
        {
            this.wordsFound = new HashSet<string>();
            this.positionsChecked = new HashSet<(int, int)>();
            this.boardHeight = boardHeight;
            this.boardWidth = boardWidth;
            this.boardLetters = boardLetters;

            StringBuilder word = new StringBuilder();

            for (int x = 0; x < boardHeight; x++) 
            {
                for (int y = 0; y < boardWidth; y++ )
                {
                    char c = boardLetters[CoordinateConvert(x, y)];
                    if (!legalWords.childs.ContainsKey(c))
                        continue;

                    (string increment, Trie nextNode) = NextCharacter(c, legalWords);
                    word.Append(increment);
                    positionsChecked.Add((x,y));
                    SearchWords(x,y, nextNode, word);
                    word.Remove(word.Length - increment.Length, increment.Length);
                    positionsChecked.Remove((x,y));
                }
            }            
            return wordsFound;
        }

        /// <summary>
        /// Search for all possible words starting from a given table index and Trie root.
        /// </summary>
        /// <param name="x">X position on the board.</param>
        /// <param name="y">Y position on the board.</param>
        /// <param name="root">The current Trie node.</param>
        /// <param name="word">The current word we are building.</param>
        private void SearchWords(int x, int y, Trie root, StringBuilder word)
        {
            // Base case found a word.
            if (root.childs.ContainsKey('*'))
                wordsFound.Add(word.ToString());

            // Looking for next characters from this letter.
            foreach ( (int pX , int pY) in this.positions )
            {
                int iX = pX + x; // Index to check..
                int iY = pY + y; // Index to check.. 

                if (positionsChecked.Contains((iX, iY))) continue;

                if (iX < boardHeight && iX >= 0 && iY < boardWidth && iY >= 0)
                {
                    char c = boardLetters[CoordinateConvert(iX, iY)];
                    if (root.childs.ContainsKey(c))
                    {
                        (string increment, Trie nextNode) = NextCharacter(c, root);
                        positionsChecked.Add((iX, iY));
                        word.Append(increment);
                        SearchWords(iX, iY, nextNode, word);
                        positionsChecked.Remove((iX, iY));
                        word.Remove(word.Length - increment.Length, increment.Length);
                    }
                }
            }
        }

        /// <summary>
        /// Finds the next increment to add to the word we are building.
        /// This handles special cases like 'Qu'.
        /// </summary>
        /// <param name="c">The current character</param>
        /// <param name="node">The current node</param>
        /// <returns>A tuple increment x node</returns>
        private (string, Trie) NextCharacter(char c, Trie node) 
        {
            if ( c == 'q'){
                node = new Trie(node);
                node = node.childs['q'];
                node = node.childs['u'];
                return ("qu", node);
            }
            else
                return ("" + c, node.childs[c]);
        }

        /// <summary>
        /// Helper function to convert x,y board cells into the boardletters index.
        /// </summary>
        /// <param name="x">The column index.</param>
        /// <param name="y">The row index.</param>
        /// <returns></returns>
        private int CoordinateConvert(int x, int y)
        {
            return (x * boardHeight) + y;
        }

        /// <summary>
        /// A simple Trie data structure.
        /// </summary>
        private class Trie
        {
            /// <summary>
            /// All childs of this trie node.
            /// </summary>
            public Dictionary<char, Trie> childs;
            
            /// <summary>
            /// The current character.
            /// </summary>
            public char Character;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="character">A character to initilize.</param>
            public Trie(char character)
            {
                childs = new Dictionary<char, Trie>();
                this.Character = character;
            }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="node">A node to clone.</param>
            public Trie(Trie node)
            {
                this.Character = node.Character;
                this.childs = node.childs;
            }

            /// <summary>
            /// Recursevely adds a word to the Trie
            /// </summary>
            /// <param name="word">The word to add</param>
            /// <param name="index">Index of the current character from the word to add. If you are adding a whole word start with 0.</param>
            public void AddWord(string word, int index)
            {
                char currentChar = index == word.Length ? '*' : word[index];
                childs.TryAdd(currentChar, new Trie(currentChar));

                // Add next character.
                if (index != word.Length)
                    childs[currentChar].AddWord(word, index + 1);
            }
        }
    }
}