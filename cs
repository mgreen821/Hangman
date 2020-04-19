using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Hangman_Game
{
    static class Generator
    {
        private static Random random = new Random(); //CREATES A NEW PRIVATE RANDOM

        public static string PickAWord(string[] words) //SELECTS A RANDOM WORD AND RETURNS THE WORD
        {

            return words[random.Next(words.Length + 1)];
        }
    }

    public class Player
    {
        private string name;
        public string Name { get => name; set => name = value; } //GET SET FOR THE PLAYERS NAME
    }

    public abstract class HostPlayer : Player //ABSTACT CLASS: PLAYER OR COMPUTER CHOOSES WORD TO GUESS
    {
        public abstract string ChooseSecretWord();
    }

    public class RealPlayer : HostPlayer
    {
        public RealPlayer(string name) //CONSTRUCTOR FOR REAL PLAYER WHICH TAKES PLAYERS NAME
        {
            Name = name;
        }

  
        public override string ChooseSecretWord() //PLAYER INPUTS A SECRET WORD TO BE GUESSED IN A MULTIPLAYER GAME
        {
            Console.WriteLine($"{Name}, please, enter the secret word: ");
            return Game.ReadTextInput();
        }
    }

    public class ComputerPlayer : HostPlayer 
    {
        const string path = "WordList.txt"; //SETS PATH TO A TXT FILE WITH A LIST OF WORDS

        public ComputerPlayer() //SETS NAME TO COMPUTER IN THE CASE OF SINGLEPLAYER MODE
        {
            this.Name = "computer";
        }

       
        public override string ChooseSecretWord() //SELECTS A RANDOM WORD FROM THE TXT FILE
        {
            string word;
            do
            {
                word = Generator.PickAWord(File.ReadAllLines(path));

                return word.Trim();
            }
            while (word != "");
        }
    }

    public class GuessingPlayer : Player
    {
        
        private int attempts = 8; //PLAYER HAS 8 GUESSING ATTEMPTS  
        private HostPlayer enemy; //
        private List<char> wrongLetters = new List<char>(); //LIST FOR WRONG LETTERS GUESSED 
        private SecretWord secretWord; //WORD THAT PLAYER IS TRYING TO GUESS
       

       
        public int Attempts { get => attempts; set => attempts = value; }
        public HostPlayer Enemy { get => enemy; set => enemy = value; }
        public SecretWord SecretWord { get => secretWord; set => secretWord = value; }


        private char ReadPlayerGuessSymbol()//READS GUESSED LETER UNTIL ENTERED CORRECTLY
        {
            char symbol;
            if (!СheckInputSymbol(out symbol)) //IF NOTHING WAS ENTERED RETURN AN ERROR
            {
                Console.WriteLine("Error. Please enter one letter");
                return ReadPlayerGuessSymbol();
            }
            if (symbol < 'a' || symbol > 'z') //IF INPUT ISNT LOWER CASE RETURN ERROR
            {
                Console.WriteLine("Error. Please use lower letters only.");
                return ReadPlayerGuessSymbol();
            }
            return symbol;
        }



        public bool TryToGuessLetter() //IF LETTER IS GUESSED CORRECTLY RETURN TRUE. OTHERWISE RETURN FALSE
        {
            bool letterGuessed = false;
            char symbol = ReadPlayerGuessSymbol(); 

            for (int i = 0; i < secretWord.Letters.Count; i++) //LOOPS OVER THE SECRET WORD TO SEE IF IT CONTAINS THE GUESSED LETTER
            {

                if (secretWord.Letters[i].Symbol == symbol)
                {
                    secretWord.Letters[i].IsGuessed = true;
                    letterGuessed = true;
                }
            }

            if (!(letterGuessed || wrongLetters.Contains(symbol)))
            {
                wrongLetters.Add(symbol);
            }

            return letterGuessed;
        }

 

  
        private string ListToString()//FORMS A STRING TO DISPLAY WRONG LETTERS
        {
            string wrongLettersString = "";
            for (int i = 0; i < wrongLetters.Count - 1; i++)
            {
                wrongLettersString += $"{wrongLetters[i]},";
            }
            if (wrongLetters.Count != 0)
                wrongLettersString += wrongLetters.Last();
            return wrongLettersString;
        }

        public static bool СheckInputSymbol(out char symbol) //CHECKS IF LETTER WAS ENTERED CORRECTLY
            => char.TryParse(Console.ReadLine(), out symbol);

        public override string ToString() //INFO ABOUT CURRENT STATE OF PLAYER 
        {
            return "============================================\n\r"
                + $"Wrong letters guessed: {ListToString()}\n\r"
                + $"{attempts} attempt(s) left\n\r"
                + "============================================\n\r";
        }
    }

        public class Letter //CLASS FOR LETTER
    {
        private char symbol;
        private bool isGuessed = false;

        public bool IsGuessed { get => isGuessed; set => isGuessed = value; }
        public char Symbol { get => symbol; set => symbol = value; }

        public Letter(char symbol)
        {
            this.symbol = symbol;
        }
    }

    public class SecretWord //CLASS FOR SECRET WORD
    {
        
        private List<Letter> letters;
        private string text;
        private bool isGuessed;
       


       
        public List<Letter> Letters { get => letters; set => letters = value; }
        public string Text { get => text; set => text = value; }
        public bool IsGuessed { get => isGuessed; set => isGuessed = value; }
        

        public SecretWord(string text)
        {
            this.Text = text;
            letters = new List<Letter>(text.Length);
        }

        public void ConvertTextToLetters()//DISECTS THE SECRET WORD AND TURNS IT INTO LETTERS
        {
            for (int i = 0; i < Text.Length; i++)
            {
                letters.Add(new Letter(Text.ToCharArray()[i]));
            }
        }

  
        public bool CheckIsGuessed()//CHECKS IF A LETTER IN THE SECRET WORD IS GUESSED BY PLAYER
        {
            isGuessed = letters.TrueForAll(l => l.IsGuessed == true);
            return isGuessed;
        }
    }

    static class Drawing
    {
        //DRAWS OVERALL IMAGE OF HANGMAN
        private static void DrawCanva(char[,] imageOfHang)
        {

            for (int i = 0; i < imageOfHang.GetLength(0); i++)
            {
                for (int j = 0; j < imageOfHang.GetLength(1); j++)
                {
                    Console.Write(imageOfHang[i, j]);
                }
                Console.WriteLine();
            }
        }

        
        private static void DrawHorPost(char[,] imageOfHang) //DRAWS HORIZONTAL POST
        {
            for (int j = 4; j < 11; j++)
            {
                imageOfHang[0, j] = '_';
            }
            imageOfHang[1, 5] = '/';
        }

        private static void DrawVerPost(char[,] imageOfHang) //DRAWS VERTICAL POST
        {
            for (int j = 1; j < 12; j++)
            {
                imageOfHang[6, j] = '_';
            }

            for (int i = 1; i < 7; i++)
            {
                imageOfHang[i, 4] = '|';
            }

            imageOfHang[6, 3] = '/';
            imageOfHang[6, 5] = '\\';
        }

        private static void DrawHeadAndRope(char[,] imageOfHang) //DRAWS HEAD AND ROPE
        {
            imageOfHang[1, 10] = '|';
            imageOfHang[2, 10] = 'O';
        }

        private static void DrawBody(char[,] imageOfHang) //DRAWS BODY
        {
            imageOfHang[3, 10] = '|';
            imageOfHang[4, 10] = '|';
        }

        

        public static void HangDrawing(int numOfMistakes, char[,] imageOfHang)//DRAWS PARTS OF HANGMAN BASED ON NUMBER OF MISTAKES
        {
            
            switch (numOfMistakes)
            {
                
                case (1):
                    {
                        DrawVerPost(imageOfHang);
                        break;
                    }
                case (2):
                    {
                        DrawHorPost(imageOfHang);
                        break;
                    }
                case (3):
                    {
                        DrawHeadAndRope(imageOfHang);
                        break;
                    }
                case (4):
                    {
                        DrawBody(imageOfHang);
                        break;
                    }
                case (5):
                    {
                        imageOfHang[3, 9] = '/';
                        break;
                    }
                case (6):
                    {
                        imageOfHang[3, 11] = '\\';
                        break;
                    }
                case (7):
                    {
                        imageOfHang[5, 9] = '/';
                        break;
                    }
                case (8):
                    {
                        imageOfHang[5, 11] = '\\';
                        break;
                    }
            }
            DrawCanva(imageOfHang);
        }

        public static void WordDrawing(SecretWord secretword)//DISPLAYS A WORD WITH UNDERSCORES TO SHOW HIDDEN LETTERS
        {
            string outputWord = "";

            foreach (var letter in secretword.Letters)
            {
                if (letter.IsGuessed) outputWord += $"{letter.Symbol} ";
                else outputWord += "_ ";
            }

            Console.WriteLine("Word:    " + outputWord);
        }
    }

    class Game
    {
       
        private int numOfPlayers;
        private char[,] imageOfHang = new char[8, 12];
        private GuessingPlayer player = new GuessingPlayer();
      

       
        public char[,] ImageOfHang { get => imageOfHang; set => imageOfHang = value; }
        public int NumOfPlayers
        {
            get => numOfPlayers;
            private set => numOfPlayers = value;
        }
        public GuessingPlayer Player { get => player; set => player = value; }
        

        
        public void CreatePlayers()//CREATES PLAYERS AND ASKS FOR THEIR NAMES
        {
            NumOfPlayers = ReadNumOfPlayers();
            Console.Clear();
            Console.WriteLine("Guessing player, please enter your name:");
            Player.Name = ReadTextInput();

            switch (this.NumOfPlayers)
            {
                case (1):
                    {
                        Player.Enemy = new ComputerPlayer();
                        break;
                    }
                case (2):
                    {
                        Console.WriteLine("Host player, please enter your name:");
                        Player.Enemy = new RealPlayer(ReadTextInput());
                        break;
                    }
            }
        }

     
        public void GuessingAction()//PLAYER'S GUESSING ACTION
        {
            Drawing.WordDrawing(Player.SecretWord);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{Player.Name}, please enter a letter:");

            if (Player.TryToGuessLetter())
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Correct!");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Player.SecretWord.CheckIsGuessed();
                if (8 - Player.Attempts > 0) Drawing.HangDrawing(8 - Player.Attempts, ImageOfHang);
                Console.WriteLine(Player);
            }
            else
            {
                if (Player.Attempts > 1)
                {
                    Player.Attempts -= 1;
                    Drawing.HangDrawing(8 - Player.Attempts, ImageOfHang);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Incorrect!");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(Player);
                }
                else
                {
                    Drawing.HangDrawing(8, ImageOfHang);
                    Program.GameOver = true;
                    EndGamePlayerFailed();
                }
            }

            if (Player.Attempts > 0 && Player.SecretWord.IsGuessed)
            {
                Program.GameOver = true;
                EndGamePlayerWin();
            }
            
            Console.WriteLine("=====================Next Guess Please=======================");
        }


        public void StartNewGame()//STARTS GAME
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("============================================\n\r"
                + "Welcome to the Hangman!\n\r"
                + "============================================");
            CreatePlayers();
            Console.Clear();
            SecretWord secretWord = new SecretWord(Player.Enemy.ChooseSecretWord());
            secretWord.ConvertTextToLetters();
            player.SecretWord = secretWord;
            Console.Clear();
        }

   
        private void EndGamePlayerWin()//ENDS GAME IF PLAYER HAS WON
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("============================================\n\r"
                + $"Congratulations! You guessed the secret word - {Player.SecretWord.Text}\n\r GAME OVER\n\r"
                + "============================================");
        }

       
        private void EndGamePlayerFailed()//ENDS GAME IF PLAYer LOSES
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("============================================\n\r"
                + "Wrong letter! You don't have any more attempts\n\r"
                + $"The secret word is {Player.SecretWord.Text}\n\r{Player.Enemy.Name} has won\n\rGAME OVER\n\r"
                + "============================================");
        }
       

        

     
        public static string ReadTextInput() //READS PLAYERS TEXT UNTIL ENTERED CORRECTLY 
        {
            string inputText = Console.ReadLine();
            if (inputText.Length == 0)
            {
                Console.WriteLine("Error. Please enter a name.");
                return ReadTextInput();
            }
            if (!CheckTextInput(inputText))
            {
                Console.WriteLine("Error. Please use lowercase only.");
                return ReadTextInput();
            }
            return inputText;
        }


        public static int ReadNumOfPlayers() //READS NUMBER OF PLAYERS UNTIL ENTERED CORRECTLY
        {
            int numOfPlayers;
            Console.WriteLine("Enter 1 for single-player mode or 2 for two-player mode.");
            while (!CheckNumOfPlayers(out numOfPlayers)) Console.WriteLine("Error. Please enter 1 or 2.");
            return numOfPlayers;
        }
       

      

        public static bool CheckTextInput(string text) //CHECK IF TEXT IS ENTERED CORRECTLY
            => Array.TrueForAll(text.ToCharArray(), s => s >= 'a' && s <= 'z');

        static bool CheckNumOfPlayers(out int numOfPlayers) //CHECK IF NUMBER IS ENTERED CORRECTLY 
            => int.TryParse(Console.ReadLine(), out numOfPlayers) && numOfPlayers > 0 && numOfPlayers < 3;
       
    }
    class Program
    {
       
        private static bool gameOver = false;
        public static bool GameOver { get => gameOver; set => gameOver = value; }
        


        static void Main(string[] args)
        {
            do
            {
                try
                {
                    Console.Clear();
                    Game game = new Game();
                    game.StartNewGame();
                    while (!gameOver)
                    {
                       
                        game.GuessingAction();
                        
                    }
                    gameOver = false;
                    Console.WriteLine("Press Enter, if you want to play again...");
                }

                catch (IOException)
                {
                    Console.WriteLine("Error reading file");
                }
             
            }
            while (Console.ReadKey(true).Key == ConsoleKey.Enter);
        }
    }
}
