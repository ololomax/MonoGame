// Generated by TinyPG v1.3 available at www.codeproject.com

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace TwoMGFX
{
    #region Scanner

    public partial class Scanner
    {
        public string Input;
        public int StartPos = 0;
        public int EndPos = 0;
        public int CurrentLine;
        public int CurrentColumn;
        public int CurrentPosition;
        public List<Token> Skipped; // tokens that were skipped
        public Dictionary<TokenType, Regex> Patterns;

        private Token LookAheadToken;
        private List<TokenType> Tokens;
        private List<TokenType> SkipList; // tokens to be skipped

        public Scanner()
        {
            Regex regex;
            Patterns = new Dictionary<TokenType, Regex>();
            Tokens = new List<TokenType>();
            LookAheadToken = null;
            Skipped = new List<Token>();

            SkipList = new List<TokenType>();
            SkipList.Add(TokenType.COMMENT);
            SkipList.Add(TokenType.COMMENTCPP);
            SkipList.Add(TokenType.WHITESPACE);

            regex = new Regex(@"/\*([^*]|\*[^/])*\*/", RegexOptions.Compiled);
            Patterns.Add(TokenType.COMMENT, regex);
            Tokens.Add(TokenType.COMMENT);

            regex = new Regex(@"//[^\n\r]*", RegexOptions.Compiled);
            Patterns.Add(TokenType.COMMENTCPP, regex);
            Tokens.Add(TokenType.COMMENTCPP);

            regex = new Regex(@"[ \t\n\r]+", RegexOptions.Compiled);
            Patterns.Add(TokenType.WHITESPACE, regex);
            Tokens.Add(TokenType.WHITESPACE);

            regex = new Regex(@"[A-Za-z_][A-Za-z0-9_]*", RegexOptions.Compiled);
            Patterns.Add(TokenType.IDENTIFIER, regex);
            Tokens.Add(TokenType.IDENTIFIER);

            regex = new Regex(@"pass", RegexOptions.Compiled);
            Patterns.Add(TokenType.PASS, regex);
            Tokens.Add(TokenType.PASS);

            regex = new Regex(@"technique", RegexOptions.Compiled);
            Patterns.Add(TokenType.TECHNIQUE, regex);
            Tokens.Add(TokenType.TECHNIQUE);

            regex = new Regex(@"VertexShader", RegexOptions.Compiled);
            Patterns.Add(TokenType.VERTEXSHADER, regex);
            Tokens.Add(TokenType.VERTEXSHADER);

            regex = new Regex(@"PixelShader", RegexOptions.Compiled);
            Patterns.Add(TokenType.PIXELSHADER, regex);
            Tokens.Add(TokenType.PIXELSHADER);

            regex = new Regex(@"{", RegexOptions.Compiled);
            Patterns.Add(TokenType.BROPEN, regex);
            Tokens.Add(TokenType.BROPEN);

            regex = new Regex(@"}", RegexOptions.Compiled);
            Patterns.Add(TokenType.BRCLOSE, regex);
            Tokens.Add(TokenType.BRCLOSE);

            regex = new Regex(@"=", RegexOptions.Compiled);
            Patterns.Add(TokenType.EQUALS, regex);
            Tokens.Add(TokenType.EQUALS);

            regex = new Regex(@";", RegexOptions.Compiled);
            Patterns.Add(TokenType.SEMICOLON, regex);
            Tokens.Add(TokenType.SEMICOLON);

            regex = new Regex(@"\(\s*\)", RegexOptions.Compiled);
            Patterns.Add(TokenType.EMPTY_ARGS, regex);
            Tokens.Add(TokenType.EMPTY_ARGS);

            regex = new Regex(@"compile", RegexOptions.Compiled);
            Patterns.Add(TokenType.COMPILE, regex);
            Tokens.Add(TokenType.COMPILE);

            regex = new Regex(@"(vs_|ps_)(2_0|3_0|4_0|5_0)", RegexOptions.Compiled);
            Patterns.Add(TokenType.SHADERMODEL, regex);
            Tokens.Add(TokenType.SHADERMODEL);

            regex = new Regex(@".", RegexOptions.Compiled);
            Patterns.Add(TokenType.CODE, regex);
            Tokens.Add(TokenType.CODE);

            regex = new Regex(@"@?\""(\""\""|[^\""])*\""", RegexOptions.Compiled);
            Patterns.Add(TokenType.STRING, regex);
            Tokens.Add(TokenType.STRING);

            regex = new Regex(@"^$", RegexOptions.Compiled);
            Patterns.Add(TokenType.EOF, regex);
            Tokens.Add(TokenType.EOF);


        }

        public void Init(string input)
        {
            this.Input = input;
            StartPos = 0;
            EndPos = 0;
            CurrentLine = 0;
            CurrentColumn = 0;
            CurrentPosition = 0;
            LookAheadToken = null;
        }

        public Token GetToken(TokenType type)
        {
            Token t = new Token(this.StartPos, this.EndPos);
            t.Type = type;
            return t;
        }

         /// <summary>
        /// executes a lookahead of the next token
        /// and will advance the scan on the input string
        /// </summary>
        /// <returns></returns>
        public Token Scan(params TokenType[] expectedtokens)
        {
            Token tok = LookAhead(expectedtokens); // temporarely retrieve the lookahead
            LookAheadToken = null; // reset lookahead token, so scanning will continue
            StartPos = tok.EndPos;
            EndPos = tok.EndPos; // set the tokenizer to the new scan position
            return tok;
        }

        /// <summary>
        /// returns token with longest best match
        /// </summary>
        /// <returns></returns>
        public Token LookAhead(params TokenType[] expectedtokens)
        {
            int i;
            int startpos = StartPos;
            Token tok = null;
            List<TokenType> scantokens;


            // this prevents double scanning and matching
            // increased performance
            if (LookAheadToken != null 
                && LookAheadToken.Type != TokenType._UNDETERMINED_ 
                && LookAheadToken.Type != TokenType._NONE_) return LookAheadToken;

            // if no scantokens specified, then scan for all of them (= backward compatible)
            if (expectedtokens.Length == 0)
                scantokens = Tokens;
            else
            {
                scantokens = new List<TokenType>(expectedtokens);
                scantokens.AddRange(SkipList);
            }

            do
            {

                int len = -1;
                TokenType index = (TokenType)int.MaxValue;
                string input = Input.Substring(startpos);

                tok = new Token(startpos, EndPos);

                for (i = 0; i < scantokens.Count; i++)
                {
                    Regex r = Patterns[scantokens[i]];
                    Match m = r.Match(input);
                    if (m.Success && m.Index == 0 && ((m.Length > len) || (scantokens[i] < index && m.Length == len )))
                    {
                        len = m.Length;
                        index = scantokens[i];  
                    }
                }

                if (index >= 0 && len >= 0)
                {
                    tok.EndPos = startpos + len;
                    tok.Text = Input.Substring(tok.StartPos, len);
                    tok.Type = index;
                }
                else if (tok.StartPos < tok.EndPos - 1)
                {
                    tok.Text = Input.Substring(tok.StartPos, 1);
                }

                if (SkipList.Contains(tok.Type))
                {
                    startpos = tok.EndPos;
                    Skipped.Add(tok);
                }
                else
                {
                    // only assign to non-skipped tokens
                    tok.Skipped = Skipped; // assign prior skips to this token
                    Skipped = new List<Token>(); //reset skips
                }
            }
            while (SkipList.Contains(tok.Type));

            LookAheadToken = tok;
            return tok;
        }
    }

    #endregion

    #region Token

    public enum TokenType
    {

            //Non terminal tokens:
            _NONE_  = 0,
            _UNDETERMINED_= 1,

            //Non terminal tokens:
            Start   = 2,
            Technique_Declaration= 3,
            Pass_Declaration= 4,
            VertexShader_Pass_Expression= 5,
            PixelShader_Pass_Expression= 6,

            //Terminal tokens:
            COMMENT = 7,
            COMMENTCPP= 8,
            WHITESPACE= 9,
            IDENTIFIER= 10,
            PASS    = 11,
            TECHNIQUE= 12,
            VERTEXSHADER= 13,
            PIXELSHADER= 14,
            BROPEN  = 15,
            BRCLOSE = 16,
            EQUALS  = 17,
            SEMICOLON= 18,
            EMPTY_ARGS= 19,
            COMPILE = 20,
            SHADERMODEL= 21,
            CODE    = 22,
            STRING  = 23,
            EOF     = 24
    }

    public class Token
    {
        private int startpos;
        private int endpos;
        private string text;
        private object value;

        // contains all prior skipped symbols
        private List<Token> skipped;

        public int StartPos { 
            get { return startpos;} 
            set { startpos = value; }
        }

        public int Length { 
            get { return endpos - startpos;} 
        }

        public int EndPos { 
            get { return endpos;} 
            set { endpos = value; }
        }

        public string Text { 
            get { return text;} 
            set { text = value; }
        }

        public List<Token> Skipped { 
            get { return skipped;} 
            set { skipped = value; }
        }
        public object Value { 
            get { return value;} 
            set { this.value = value; }
        }

        [XmlAttribute]
        public TokenType Type;

        public Token()
            : this(0, 0)
        {
        }

        public Token(int start, int end)
        {
            Type = TokenType._UNDETERMINED_;
            startpos = start;
            endpos = end;
            Text = ""; // must initialize with empty string, may cause null reference exceptions otherwise
            Value = null;
        }

        public void UpdateRange(Token token)
        {
            if (token.StartPos < startpos) startpos = token.StartPos;
            if (token.EndPos > endpos) endpos = token.EndPos;
        }

        public override string ToString()
        {
            if (Text != null)
                return Type.ToString() + " '" + Text + "'";
            else
                return Type.ToString();
        }
    }

    #endregion
}
