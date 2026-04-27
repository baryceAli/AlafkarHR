namespace SharedWithUI.Employees.Enums;

public static class NationalityTranslator
{
    public static string GetDisplay(Nationality nationality, string lang = "En")
    {
        return nationality switch
        {
            // GCC
            Nationality.SaudiArabian => lang == "Ar" ? "سعودي" : "Saudi",
            Nationality.Emirati => lang == "Ar" ? "إماراتي" : "Emirati",
            Nationality.Kuwaiti => lang == "Ar" ? "كويتي" : "Kuwaiti",
            Nationality.Qatari => lang == "Ar" ? "قطري" : "Qatari",
            Nationality.Bahraini => lang == "Ar" ? "بحريني" : "Bahraini",
            Nationality.Omani => lang == "Ar" ? "عماني" : "Omani",

            // Middle East
            Nationality.Egyptian => lang == "Ar" ? "مصري" : "Egyptian",
            Nationality.Jordanian => lang == "Ar" ? "أردني" : "Jordanian",
            Nationality.Palestinian => lang == "Ar" ? "فلسطيني" : "Palestinian",
            Nationality.Lebanese => lang == "Ar" ? "لبناني" : "Lebanese",
            Nationality.Syrian => lang == "Ar" ? "سوري" : "Syrian",
            Nationality.Iraqi => lang == "Ar" ? "عراقي" : "Iraqi",
            Nationality.Yemeni => lang == "Ar" ? "يمني" : "Yemeni",

            // North Africa
            Nationality.Sudanese => lang == "Ar" ? "سوداني" : "Sudanese",
            Nationality.Libyan => lang == "Ar" ? "ليبي" : "Libyan",
            Nationality.Tunisian => lang == "Ar" ? "تونسي" : "Tunisian",
            Nationality.Algerian => lang == "Ar" ? "جزائري" : "Algerian",
            Nationality.Moroccan => lang == "Ar" ? "مغربي" : "Moroccan",

            // South Asia
            Nationality.Indian => lang == "Ar" ? "هندي" : "Indian",
            Nationality.Pakistani => lang == "Ar" ? "باكستاني" : "Pakistani",
            Nationality.Bangladeshi => lang == "Ar" ? "بنغلاديشي" : "Bangladeshi",
            Nationality.SriLankan => lang == "Ar" ? "سريلانكي" : "Sri Lankan",
            Nationality.Nepalese => lang == "Ar" ? "نيبالي" : "Nepalese",
            Nationality.Borma=> lang=="Ar"?"بورماوي": "Borma",

            // Southeast Asia
            Nationality.Filipino => lang == "Ar" ? "فلبيني" : "Filipino",
            Nationality.Indonesian => lang == "Ar" ? "إندونيسي" : "Indonesian",
            Nationality.Malaysian => lang == "Ar" ? "ماليزي" : "Malaysian",
            Nationality.Thai => lang == "Ar" ? "تايلاندي" : "Thai",
            Nationality.Vietnamese => lang == "Ar" ? "فيتنامي" : "Vietnamese",

            // East Asia
            Nationality.Chinese => lang == "Ar" ? "صيني" : "Chinese",
            Nationality.Japanese => lang == "Ar" ? "ياباني" : "Japanese",
            Nationality.Korean => lang == "Ar" ? "كوري" : "Korean",

            // Europe
            Nationality.British => lang == "Ar" ? "بريطاني" : "British",
            Nationality.French => lang == "Ar" ? "فرنسي" : "French",
            Nationality.German => lang == "Ar" ? "ألماني" : "German",
            Nationality.Italian => lang == "Ar" ? "إيطالي" : "Italian",
            Nationality.Spanish => lang == "Ar" ? "إسباني" : "Spanish",
            Nationality.Portuguese => lang == "Ar" ? "برتغالي" : "Portuguese",
            Nationality.Dutch => lang == "Ar" ? "هولندي" : "Dutch",
            Nationality.Belgian => lang == "Ar" ? "بلجيكي" : "Belgian",
            Nationality.Swedish => lang == "Ar" ? "سويدي" : "Swedish",
            Nationality.Norwegian => lang == "Ar" ? "نرويجي" : "Norwegian",
            Nationality.Danish => lang == "Ar" ? "دنماركي" : "Danish",
            Nationality.Finnish => lang == "Ar" ? "فنلندي" : "Finnish",
            Nationality.Russian => lang == "Ar" ? "روسي" : "Russian",
            Nationality.Ukrainian => lang == "Ar" ? "أوكراني" : "Ukrainian",
            Nationality.Polish => lang == "Ar" ? "بولندي" : "Polish",
            Nationality.Greek => lang == "Ar" ? "يوناني" : "Greek",
            Nationality.Turkish => lang == "Ar" ? "تركي" : "Turkish",

            // Americas
            Nationality.American => lang == "Ar" ? "أمريكي" : "American",
            Nationality.Canadian => lang == "Ar" ? "كندي" : "Canadian",
            Nationality.Mexican => lang == "Ar" ? "مكسيكي" : "Mexican",
            Nationality.Brazilian => lang == "Ar" ? "برازيلي" : "Brazilian",
            Nationality.Argentinian => lang == "Ar" ? "أرجنتيني" : "Argentinian",
            Nationality.Chilean => lang == "Ar" ? "تشيلي" : "Chilean",
            Nationality.Colombian => lang == "Ar" ? "كولومبي" : "Colombian",
            Nationality.Peruvian => lang == "Ar" ? "بيروفي" : "Peruvian",

            // Africa
            Nationality.Nigerian => lang == "Ar" ? "نيجيري" : "Nigerian",
            Nationality.Kenyan => lang == "Ar" ? "كيني" : "Kenyan",
            Nationality.Ethiopian => lang == "Ar" ? "إثيوبي" : "Ethiopian",
            Nationality.Ugandan => lang == "Ar" ? "أوغندي" : "Ugandan",
            Nationality.Tanzanian => lang == "Ar" ? "تنزاني" : "Tanzanian",
            Nationality.SouthAfrican => lang == "Ar" ? "جنوب أفريقي" : "South African",
            Nationality.Ghanaian => lang == "Ar" ? "غاني" : "Ghanaian",

            // Others
            Nationality.Australian => lang == "Ar" ? "أسترالي" : "Australian",
            Nationality.NewZealander => lang == "Ar" ? "نيوزيلندي" : "New Zealander",

            _ => nationality.ToString()
        };
    }
}