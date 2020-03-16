using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace PrimeTech.Core
{
    public class Language
    {
        public static readonly Language Turkish = new Language("tr");
        public static readonly Language English = new Language("en");
        public static readonly Language Afrikaans = new Language("af");
        public static readonly Language Albanian = new Language("sq");
        public static readonly Language Amharic = new Language("am");
        public static readonly Language Arabic = new Language("ar");
        public static readonly Language Armenian = new Language("hy");
        public static readonly Language Azerbaijani = new Language("az");
        public static readonly Language Bashkir = new Language("ba");
        public static readonly Language Belarusian = new Language("be");
        public static readonly Language Basque = new Language("eu");
        public static readonly Language Bengali = new Language("bn");
        public static readonly Language Bulgarian = new Language("bg");
        public static readonly Language Burmese = new Language("my");
        public static readonly Language Bosnian = new Language("bs");
        public static readonly Language EnglishUK = new Language("en-GB");
        public static readonly Language PortugueseBrazil = new Language("pt-BR");
        public static readonly Language Catalan = new Language("ca");
        public static readonly Language Cebuano = new Language("ceb");
        public static readonly Language Cherokee = new Language("chr");
        public static readonly Language Croatian = new Language("hr");
        public static readonly Language Czech = new Language("cs");
        public static readonly Language Danish = new Language("da");
        public static readonly Language Dutch = new Language("nl");
        public static readonly Language Estonian = new Language("et");
        public static readonly Language Esperanto = new Language("eo");
        public static readonly Language Filipino = new Language("fil");
        public static readonly Language Finnish = new Language("fi");
        public static readonly Language French = new Language("fr");
        public static readonly Language Galician = new Language("gl");
        public static readonly Language German = new Language("de");
        public static readonly Language Georgian = new Language("ka");
        public static readonly Language Greek = new Language("el");
        public static readonly Language Gujarati = new Language("gu");
        public static readonly Language Haitian = new Language("ht");
        public static readonly Language Hebrew = new Language("iw");
        public static readonly Language Hindi = new Language("hi");
        public static readonly Language Hungarian = new Language("hu");
        public static readonly Language HillMari = new Language("mrj");
        public static readonly Language Icelandic = new Language("is");
        public static readonly Language Indonesian = new Language("id");
        public static readonly Language Irish = new Language("ga");
        public static readonly Language Italian = new Language("it");
        public static readonly Language Japanese = new Language("ja");
        public static readonly Language Javanese = new Language("jv");
        public static readonly Language Kannada = new Language("kn");
        public static readonly Language Kazakh = new Language("kk");
        public static readonly Language Khmer = new Language("km");
        public static readonly Language Kyrgyz = new Language("ky");
        public static readonly Language Korean = new Language("ko");
        public static readonly Language Latvian = new Language("lv");
        public static readonly Language Latin = new Language("la");
        public static readonly Language Lithuanian = new Language("lt");
        public static readonly Language Luxembourgish = new Language("lb");
        public static readonly Language Lao = new Language("lo");
        public static readonly Language Malay = new Language("ms");
        public static readonly Language Macedonian = new Language("mk");
        public static readonly Language Mari = new Language("mhr");
        public static readonly Language Maori = new Language("mi");
        public static readonly Language Malagasy = new Language("mg");
        public static readonly Language Malayalam = new Language("ml");
        public static readonly Language Marathi = new Language("mr");
        public static readonly Language Mongolian = new Language("mn");
        public static readonly Language Maltese = new Language("mt");
        public static readonly Language Norwegian = new Language("no");
        public static readonly Language Nepali = new Language("ne");
        public static readonly Language Persian = new Language("fa");
        public static readonly Language Polish = new Language("pl");
        public static readonly Language Punjabi = new Language("pa");
        public static readonly Language Papiamento = new Language("pap");
        public static readonly Language Portuguese = new Language("pt-PT");
        public static readonly Language Romanian = new Language("ro");
        public static readonly Language Russian = new Language("ru");
        public static readonly Language Serbian = new Language("sr");
        public static readonly Language Chinese = new Language("zh-CN");
        public static readonly Language ScottishGaelic = new Language("gd");
        public static readonly Language Sinhalese = new Language("si");
        public static readonly Language Slovak = new Language("sk");
        public static readonly Language Slovenian = new Language("sl");
        public static readonly Language Spanish = new Language("es");
        public static readonly Language Sundanese = new Language("su");
        public static readonly Language Swahili = new Language("sw");
        public static readonly Language Swedish = new Language("sv");
        public static readonly Language Tagalog = new Language("tl");
        public static readonly Language Tamil = new Language("ta");
        public static readonly Language Telugu = new Language("te");
        public static readonly Language Tajik = new Language("tg");
        public static readonly Language Thai = new Language("th");
        public static readonly Language Tatar = new Language("tt");
        public static readonly Language ChineseTaiwan = new Language("zh-TW");
        public static readonly Language Udmurt = new Language("udm");
        public static readonly Language Urdu = new Language("ur");
        public static readonly Language Ukrainian = new Language("uk");
        public static readonly Language Uzbek = new Language("uz");
        public static readonly Language Vietnamese = new Language("vi");
        public static readonly Language Xhosa = new Language("xh");
        public static readonly Language Yiddish = new Language("yi");
        public static readonly Language Welsh = new Language("cy");
        private static List<Language> allLanguages = null;

        private readonly string code;

        private Language(string code)
        {
            this.code = code;
        }

        public override string ToString()
        {
            return this.code;
        }

        public static List<Language> GetAllLanguages()
        {
            if (allLanguages == null)
            {
                allLanguages = typeof(Language).GetFields(BindingFlags.Public | BindingFlags.Static).Select(x => (Language)x.GetValue(null)).ToList();
            }
            return allLanguages;
        }

    }
}