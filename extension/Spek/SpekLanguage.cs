using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricSky.Spek
{
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Package;
    using Microsoft.VisualStudio.TextManager.Interop;

    public class SpekLanguageService : LanguageService
    {
        private LanguagePreferences _preferences;

        private SpekScanner _scanner;

        public override string GetFormatFilterList()
        {
            throw new NotImplementedException();
        }

        public override LanguagePreferences GetLanguagePreferences()
        {
            if (this._preferences == null)
            {
                this._preferences = new LanguagePreferences(this.Site,
                                                        typeof(SpekLanguageService).GUID,
                                                        this.Name);
                this._preferences.Init();
            }
            return this._preferences;
        }

        public override IScanner GetScanner(IVsTextLines buffer)
        {
            if (this._scanner == null)
            {
                this._scanner = new SpekScanner(buffer);
            }
            return this._scanner;
        }

        public override string Name
        {
            get
            {
                return "Spek";
            }
        }

        public override AuthoringScope ParseSource(ParseRequest req)
        {
            return new SpekAuthoringScope();
        }
    }

    public class SpekAuthoringScope : AuthoringScope
    {
        public override string GetDataTipText(int line, int col, out TextSpan span)
        {
            span = new TextSpan();
            return null;
        }

        public override Declarations GetDeclarations(IVsTextView view, int line, int col, TokenInfo info, ParseReason reason)
        {
            return null;
        }

        public override string Goto(VSConstants.VSStd97CmdID cmd, IVsTextView textView, int line, int col, out TextSpan span)
        {
            span = new TextSpan();
            return null;
        }

        public override Methods GetMethods(int line, int col, string name)
        {
            return null;
        }
    }

    internal class SpekScanner : IScanner
    {
        private IVsTextBuffer _buffer;
        string _source;

        public SpekScanner(IVsTextBuffer buffer)
        {
            this._buffer = buffer;
        }

        bool IScanner.ScanTokenAndProvideInfoAboutIt(TokenInfo tokenInfo, ref int state)
        {
            tokenInfo.Type = TokenType.Unknown;
            tokenInfo.Color = TokenColor.Text;
            return true;
        }

        void IScanner.SetSource(string source, int offset)
        {
            this._source = source.Substring(offset);
        }
    }
}
