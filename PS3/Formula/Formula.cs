﻿// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax; variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        ///An array of strings containing the normalized formula for this object. Populated during construction.
        List<string> normalizedFormula;

        ///A set containing valid arithmetic operators. Done to shorten conditionals. 
        HashSet<string> validOps;
        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string,string> normalize, Func<string,bool> isValid)
        {
            
            normalizedFormula = new List<string>(); //Initialize our list of tokens.

            validOps = new HashSet<string>(); //Initialize our valid operators.
            validOps.Add("+");
            validOps.Add("-");
            validOps.Add("*");
            validOps.Add("/");

            double currentDouble; //If we're working with a double when looking at a specific token, it's stored here.

            int openParenSoFar = 0; //The paren pairs we've seen so far.
            int closedParenSoFar = 0;

            //Remembers if the last token was a number, variable, or a close paren.
            //If this is false, the last token was an open paren, or an operator.
            bool lastTokenNumVarCloseParen; 

            IEnumerator<string> tokens = GetTokens(formula).GetEnumerator();

            //Code for the first element
            if (!tokens.MoveNext())
            {
                throw new FormulaFormatException("You must have at least one token");
            }
            else
            {
                if (!(Double.TryParse(tokens.Current, out currentDouble)  || IsVariable(tokens.Current) || tokens.Current.Equals("(")))
                {
                    throw new FormulaFormatException("The formula must start with a variable, a number, or an open parenthesis. " + 
                                                        "It starts with:" + tokens.Current);
                }
                else
                {
                    //Case for variables.
                    if (IsVariable(tokens.Current))
                    {
                        lastTokenNumVarCloseParen = true;
                        string normalizedToken = normalize(tokens.Current);
                        if (isValid(normalizedToken) && IsVariable(normalizedToken))
                        {
                            normalizedFormula.Add(normalizedToken);
                        }
                        else
                        {
                            throw new FormulaFormatException("Normalized token is not valid:" + normalizedToken);
                        }
                    } else if (tokens.Current.Equals("(")) { //case for open parenthesis
                        lastTokenNumVarCloseParen = false;
                        openParenSoFar++;
                        normalizedFormula.Add(tokens.Current);
                        
                    }
                    else
                    {
                        //If we get to this point, it's a double.
                        lastTokenNumVarCloseParen = true;
                        normalizedFormula.Add(currentDouble.ToString());
                    }
                }
            }

            int tokenListLengthStart; //The length of the token list at the beginning of each loop. Declared her to avoid allocation overhead.
            while (tokens.MoveNext())
            {
                tokenListLengthStart = normalizedFormula.Count;

                if (tokens.Current.Equals(")"))
                {
                    //If the previous element was an op or open paren, throw.
                    if (!lastTokenNumVarCloseParen)
                    {
                        throw new FormulaFormatException("You have a close paren following an operator or open paren.");
                    }

                    closedParenSoFar++;
                    if (closedParenSoFar > openParenSoFar)
                    {
                        throw new FormulaFormatException("You have a close paren without an opening one.");
                    }
                    lastTokenNumVarCloseParen = true;
                    normalizedFormula.Add(tokens.Current);
                }

                if (tokens.Current.Equals("("))
                {
                    //If the previous element was a var, num, or close paren, throw.
                    if (lastTokenNumVarCloseParen)
                    {
                        throw new FormulaFormatException("You have an open paren following a close paren, variable, or number.");
                    }
                    openParenSoFar++;
                    lastTokenNumVarCloseParen = false;
                    normalizedFormula.Add(tokens.Current);
                }

                if (IsVariable(tokens.Current))
                {
                    if (lastTokenNumVarCloseParen)
                    {
                        throw new FormulaFormatException("You have a variable following another number, variable or close paren.");
                    }
                    string normalizedToken = normalize(tokens.Current);
                    if (isValid(normalizedToken) && IsVariable(normalizedToken))
                    {
                        lastTokenNumVarCloseParen = true;
                        normalizedFormula.Add(normalizedToken);
                    }
                    else
                    {
                        throw new FormulaFormatException("Normalized token is not valid:" + normalizedToken);
                    }
                }

                if (Double.TryParse(tokens.Current, out currentDouble))
                {
                    if (lastTokenNumVarCloseParen)
                    {
                        throw new FormulaFormatException("You have a number following another number, variable or close paren.");
                    }
                    lastTokenNumVarCloseParen = true;
                    normalizedFormula.Add(currentDouble.ToString());
                }

                if (validOps.Contains(tokens.Current))
                {
                    if (!lastTokenNumVarCloseParen)
                    {
                        throw new FormulaFormatException("You have an operator following another operator, or an open paren.");
                    }
                    lastTokenNumVarCloseParen = false;
                    normalizedFormula.Add(tokens.Current);
                }

                //If we haven't added a new token by this point, then we encountered an invalid token, and we need to throw
                if (tokenListLengthStart == normalizedFormula.Count)
                {
                    throw new FormulaFormatException("Invalid Tokens:" + tokens.Current);
                }
            }

            //The number of open parenthesis has to match the number of close ones:
            if (openParenSoFar != closedParenSoFar)
            {
                throw new FormulaFormatException("Number of open parenthisis need to match the number of close parenthisis");
            }

            //The last token can only be a number, variable or close parenthisis
            if (!lastTokenNumVarCloseParen)
            {
                throw new FormulaFormatException("Invalid value for final token:" + tokens.Current);
            }

        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            return null;
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            return null;
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            return null;
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens, which are compared as doubles, and variable tokens,
        /// whose normalized forms are compared as strings.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object obj)
        {
            return false;
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            return false;
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            return false;
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            return 0;
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }
        /// <summary>
        /// Returns true if the string passed to this function is a valid variable (IE: Is a letter or underscore 
        /// followed by zero or more letters, digits, or underscores.
        /// </summary>
        /// <param name="variablename"> The string to check.</param>
        /// <returns>True if the the string passed was a valid variable name, false if not.</returns>
        private static bool IsVariable(String varname)
        {
            if (!(varname[0].Equals("_") || Char.IsLetter(varname[0])))
            {
                return false; //The variable must start with an underscore or letter.
            }

            for (int i = 1; i < varname.Length; i++)
            {
                if (!(varname[0].Equals("_") || Char.IsLetterOrDigit(varname[0])))
                {
                    return false; //Return false if we encounter anything that's not a letter, digit, or underscore.
                }
            }
            
            return true; //If we haven't broken the rules, its valid.
        }

    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
}