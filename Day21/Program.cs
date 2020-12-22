﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Day21
{
    class Program
    {
        static void Main(string[] args)
        {
            Part1Part2(PROBLEM);
        }

        private static void Part1Part2(string input)
        {
            var foods = input
                .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Select(x =>
                {
                    var temp = x.Split(" (contains");
                    var ingredients = temp[0].Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    var allergens = temp[1]
                    .Split(")", StringSplitOptions.RemoveEmptyEntries)[0]
                    .Split(",", StringSplitOptions.RemoveEmptyEntries)
                    .Select(e => e.Trim()).ToArray();
                    return new Food() { Ingredients = ingredients, Allergens = allergens };
                }).ToArray();

            var ingredientToAllergen = new Dictionary<string, string>();
            var allergenToIngredient = new Dictionary<string, string>();

            var allergenToIngredientCandidateList = foods.SelectMany(e => e.Allergens).Distinct().ToDictionary(e => e, e => foods.SelectMany(g => g.Ingredients).Distinct().ToList());

            // Cross all combinations
            bool progress = true;
            while (progress)
            {
                progress = false;
                for (int i = 0; i < foods.Length; i++)
                {
                    for (int j = i + 1; j < foods.Length; j++)
                    {
                        // Start by removing any ingredients we know about
                        var foodsIIngs = foods[i].Ingredients.Where(x => !ingredientToAllergen.ContainsKey(x)).ToArray();
                        var foodsIAlls = foods[i].Allergens.Where(x => !allergenToIngredient.ContainsKey(x)).ToArray();

                        var foodsJIngs = foods[j].Ingredients.Where(x => !ingredientToAllergen.ContainsKey(x)).ToArray();
                        var foodsJAlls = foods[j].Allergens.Where(x => !allergenToIngredient.ContainsKey(x)).ToArray();

                        var commonIngredients = foodsIIngs.Intersect(foodsJIngs).ToArray();
                        var commonAllergens = foodsIAlls.Intersect(foodsJAlls).ToArray();


                        //else if (commonAllergens.Count() == 0 && commonIngredients.Count() > 0)
                        //{
                        //    progress = true;
                        //    foreach (var ing in commonIngredients)
                        //    {
                        //        // No way this is an allergen
                        //        ingredientToAllergen[ing] = null;
                        //    }
                        //}

                        //if (foodsIAlls.Length == 0 && commonAllergens.Length == 0 && foodsIIngs.Length > 0)
                        //{
                        //    progress = true;
                        //    foreach (var ing in foodsIIngs)
                        //    {
                        //        // No way this is an allergen
                        //        ingredientToAllergen[ing] = null;
                        //    }
                        //}

                        //if (foodsJAlls.Length == 0 && commonAllergens.Length == 0 && foodsJIngs.Length > 0)
                        //{
                        //    progress = true;
                        //    foreach (var ing in foodsJIngs)
                        //    {
                        //        // No way this is an allergen
                        //        ingredientToAllergen[ing] = null;
                        //    }
                        //}

                        // Easy match
                        if (commonAllergens.Length == 1 && commonIngredients.Length == 1)
                        {
                            progress = true;
                            ingredientToAllergen[commonIngredients.First()] = commonAllergens.First();
                            allergenToIngredient[commonAllergens.First()] = commonIngredients.First();
                        }
                        if (foodsIAlls.Length == 1 && foodsIIngs.Length == 1)
                        {
                            progress = true;
                            allergenToIngredient[foodsIAlls[0]] = foodsIIngs[0];
                            ingredientToAllergen[foodsIIngs[0]] = foodsIAlls[0];
                        }
                        if (foodsJAlls.Length == 1 && foodsJIngs.Length == 1)
                        {
                            progress = true;
                            allergenToIngredient[foodsJAlls[0]] = foodsJIngs[0];
                            ingredientToAllergen[foodsJIngs[0]] = foodsJAlls[0];
                        }

                        // Allergen logic
                        var commonIngredientsSet = new HashSet<string>(commonIngredients);
                        var uniqueIngredientsI = new HashSet<string>(foodsIIngs.Except(commonIngredients));
                        var uniqueIngredientsJ = new HashSet<string>(foodsJIngs.Except(commonIngredients));

                        foreach (var allergen in commonAllergens)
                        {
                            // These allergens can only be matched to common ingredients
                            if (allergenToIngredientCandidateList[allergen].RemoveAll(e => !commonIngredientsSet.Contains(e)) > 0)
                                progress = true;
                        }                        

                        foreach (var allergen in foodsIAlls.Except(commonAllergens))
                        {
                            // These allergens must be somewhere in food is list
                            if (allergenToIngredientCandidateList[allergen].RemoveAll(e => !uniqueIngredientsI.Contains(e) && !commonIngredientsSet.Contains(e)) > 0)
                                progress = true;
                        }

                        foreach (var allergen in foodsJAlls.Except(commonAllergens))
                        {
                            // These allergens must be somewhere in food is list
                            if (allergenToIngredientCandidateList[allergen].RemoveAll(e => !uniqueIngredientsJ.Contains(e) && !commonIngredientsSet.Contains(e)) > 0)
                                progress = true;
                        }
                    }

                    foreach (KeyValuePair<string, List<string>> allergenList in allergenToIngredientCandidateList)
                    {
                        // We've reduced it to 1
                        if (allergenList.Value.Count == 1 && !allergenToIngredient.ContainsKey(allergenList.Key))
                        {
                            progress = true;
                            allergenToIngredient[allergenList.Key] = allergenList.Value.First();
                            ingredientToAllergen[allergenList.Value.First()] = allergenList.Key;
                        }
                    }
                }
            }

            //if (ingredientToAllergen.Count != foods.SelectMany(e => e.Ingredients).Distinct().Count())
            //{
            //    Console.WriteLine("Didnt find all ingredient allergen pairs");
            //    throw new Exception();
            //}

            Console.WriteLine($"Answer is {foods.SelectMany(e => e.Ingredients).Where(e => !ingredientToAllergen.ContainsKey(e) || ingredientToAllergen[e] == null).Count()}");
            Console.WriteLine($"Answer is {string.Join(',',allergenToIngredient.OrderBy(e => e.Key).Select(e => e.Value))}");

            Console.WriteLine();
        }

        class Food
        {
            public string[] Ingredients { get; set; }
            public string[] Allergens { get; set; }
        }
        

        const string EXAMPLE = @"mxmxvkd kfcds sqjhc nhms (contains dairy, fish)
trh fvjkl sbzzf mxmxvkd (contains dairy)
sqjhc fvjkl (contains soy)
sqjhc mxmxvkd sbzzf (contains fish)";


        const string PROBLEM = @"nncl snvzl jsrrk grbmvx qzczl zh kbbmsc bltrrnl jkpmsf hfxss shjvxj lnrjbft bgbvr kdvrrmmx kdqls sgslkt jlzhsl zdl pnhd vsv zgbh dlhx thbdr nmzz bmmb hkrnqnlt qfbh zrzh ghrnz nbdcsj gffbft shpglbr bgfdt jfjr pltnp dbxkrn kxjt hzc rhzgdm hdvkmd ggdbl dsvz ldpr scms rrqtp glc ddkk pgvqd qgqlv dcfj drrzq xncmd nnbn mgczn plhpz gcp zsfzq pbtq kbxcj pmzcrrd sgkmrlt frpvd nvhnjb cmqs zvd frtdmqzb scg xknmp gdnhx sxpv qzmnp ddtkx hbgsgt zppsxq gjmvxg tjlgcb zfcqk lfspxch qhrd qjvf kktsjbh gjjglc gbbs lcggks lsrxkjt gpzch xpqt bjgsll txqrj rgklhlj qcrdn (contains nuts, soy)
kdvrrmmx bgrrld xxltvc pvmj sdkvkg rzh ggdbl bgfdt vtcj vctgp tbfxdtm qqfr grbmvx zfcqk kxjt txqrj mqbv qjvf zgbh hbgsgt vpfnsz ngxmvj nrdz zfmx lqrl gxpmk mxrs zrzh sgskhk shpglbr nmzz jkpmsf trmkt mtdnt kvghnj kbbmsc phszhl scms mdtvbb kktsjbh zcqrgk qdmr rhzbf ggjslpt qhrd flfsv mgczn frpvd kdqls jsrrk fbbgv hnhrv vkxrzx hgn pnhd dcfp cdr scg fpqqbv dcfj bmmb sxpv thbdr pvhs cnmdp mcjbn nnbn frtdmqzb dzjhj vqh (contains fish, dairy)
vckt zsfzq vtbx qtgks pltnp bgfdt cdr pbtq jlzhsl txhs xncmd mvxdcbm kktsjbh rrqtp cnmdp kvk zfmx mcjbn hfxss nrktps lnrjbft nbdcsj grbmvx vtcj pgvqd zgbh rmvqb dzjhj gbbs pstspq qzczl tppntfp jgbmk qcrdn rnllc lsrxkjt kbj lthn mgczn gjmvxg mglzsz zstc zh zfcqk rzh kdqls qjvf vzsjd qqfr bgbvr tbfxdtm ddtkx bjgsll dzptlj dqcpjp xnrbnl zmtj ldpr jm phszhl kbbmsc ggjslpt hgn kbxcj vsxcjpp fpqqbv ggdbl mqbv xknmp ghrnz mdtvbb (contains dairy, soy)
drkgc hjhknn lthn rzh sdkvkg qgqlv nmzz fsdgvh dbxkrn zcqrgk mvxdcbm kdvrrmmx vsxcjpp zfmx gdzkp glc mndpbq xknmp dzjhj mdtvbb ggdbl zh gcp zmtj flfsv vzsjd mcjbn hkrnqnlt ldpr kxjt frtdmqzb kbj drrzq sgskhk rgklhlj kbxcj zdl mqbv bjgsll mlbp ghrnz jsrrk mgczn xpqt nvhnjb kvjr nncl tppntfp tzjvdj dlhx dlpvq htmkhr zjpzr vtbx frpvd dmtfl tjlgcb hnhrv zsfzq kvk lcggks cmqs shjvxj xvqbk grbmvx zstc mtdnt kdqls xncmd phszhl zfcqk lsrxkjt shpglbr bjqnjv bgrrld (contains nuts, sesame)
dzptlj nncl lfrcfd zsfzq gpbzq nrdz zppsxq jlzhsl gjmvxg ggdbl xvqbk dsvz lfzts hbgsgt grzznd drkgc xknmp rrqtp dcfj gxpmk tppntfp qhrd tbfxdtm gdzkp zfmx fpqqbv kktsjbh mdtvbb qzmnp kvghnj vctgp mlbp tkhkf pvhs dpclg kdqls mgczn xkrm zcqrgk xpgnz zfcqk mcjbn bgrrld rhzgdm qtgks zmtj gpzch rnllc zjpzr zvd vqh bjgsll vzsjd ggjslpt qqfr jfjr jm qjvf pgvqd lnrjbft ghrnz vkxrzx zrzh (contains wheat)
zjpzr dcfp mndpbq pmzcrrd plhpz thbdr jfjr drrzq gdzkp rzh pfnv gbbs grbmvx qgqlv kdvrrmmx zstc mdtvbb xncmd zsfzq bjgsll mbvpv kbxcj bgrrld vtcj frpvd ggdbl rrqtp nmzz zdl gpn kvjr lsrxkjt vsxcjpp kktsjbh xflfvx dcfj dzjhj mcjbn gjjglc gffbft vkxrzx vpfnsz svkjz gpbzq hbgsgt zcqrgk mqbv rnllc vctgp mgczn vckt mglzsz dlpvq lnrjbft nrdz zrzh gjmvxg trmkt xpgnz sdkvkg kdqls hkrnqnlt fpqqbv grzznd cnmdp jsjx cdr xpqt nbdcsj jlzhsl gcp tppntfp lfspxch xnrbnl frtdmqzb fbbgv (contains peanuts, fish, nuts)
xflfvx kvk vtbx pmzcrrd dmtfl xkrm qqfr vctgp cdr gdzkp gcp snvzl hnhrv bgbvr dcfp dbxkrn svkjz kxjt mglzsz zstc zfcqk kktsjbh xnrbnl gjjglc plhpz gxpmk scg trmkt pltnp frpvd sgkmrlt kdqls qjvf xknmp jlzhsl lsrxkjt mgczn lfspxch qgqlv nrdz zcqrgk ddkk qdmr nrktps jsrrk fsdgvh pnhd hfxss mdtvbb tjlgcb ggdbl dsvz tjrrm rhzbf zh fgfzb jm rgklhlj dqcpjp ghrnz (contains wheat, sesame, dairy)
zsfzq kvk tkhkf ddkk ggdbl zrzh lfspxch vkxrzx cnmdp kbj dpclg rnllc mlbp sxpv htmkhr nmzz xfkhp dmtfl vsv tzjvdj zstc gxpmk nncl lfrcfd zcqrgk dcfp bgrrld flfsv shpglbr ggjslpt vtbx ldpr xlzlv vmj jm bgfdt ngxmvj pfnv hjhknn kvghnj nbdcsj jgbmk mdtvbb zppsxq vqh scms zfcqk mgczn bgbvr rmvqb nnbn dcfj vfcppm ghrnz nvhnjb cmqs phszhl kktsjbh pgvqd dzptlj tppntfp pnhd pvmj gpbzq scg qfbh qgqlv snvzl grzznd kdqls (contains fish, sesame)
nnbn tjrrm frpvd vckt qfbh vfcppm zcqrgk flfsv jsjx ggdbl kvk gjjglc mdtvbb gpn txmjl tbfxdtm ghrnz sxpv dpclg zmtj fsdgvh xknmp dsvz vzsjd zfcqk lfzts pvhs bgfdt jm rhzgdm lthn ddkk phszhl scms kvjr jsrrk thbdr mqbv txqrj pfnv hfxss kktsjbh bgrrld fbbgv rgklhlj qdmr mndpbq grzznd drkgc xlzlv nncl qgqlv shmxg kdqls mgczn (contains nuts)
thbdr rgklhlj frpvd qcrdn pltnp svkjz ggjslpt shmxg gpn dmtfl vsxcjpp lthn vqh pmzcrrd vfcppm qzczl grzznd fpqqbv ldpr ggdbl ddkk tjrrm vtcj pdxcdlq zjpzr kvjr bmmb gjjglc xnrbnl xxltvc zfmx xpqt vtbx lnrjbft shjvxj xknmp dqcpjp mcjbn dsvz vzsjd mdtvbb mgczn gxpmk hzc kbxcj pbtq qfbh dcfj pvmj lcggks zsfzq hfxss xkrm dzptlj qqfr kdqls zfcqk qgqlv gpzch vckt nrktps mvxdcbm drrzq fbbgv (contains fish, peanuts)
jsrrk lsrxkjt rjhs vpfnsz zfcqk fbbgv qjvf trmkt mgczn mdtvbb mqhl bjqnjv mvxdcbm tbfxdtm xncmd fpqqbv frpvd pvmj mqbv zjpzr qhrd gjjglc zdl dmtfl zsfzq hgn smspv sgslkt zfmx mcjbn dlpvq pgvqd xflfvx mbvpv dzjhj cmqs bgfdt ghrnz rhzbf hkrnqnlt hzc dqcpjp flfsv qfbh zvd kktsjbh kdqls sgskhk scg (contains soy)
zfcqk kdqls drkgc fpqqbv nmzz zfmx zrzh grzznd trmkt kbj gdnhx zgbh gdzkp jsjx ggdbl dlpvq jgbmk tjrrm gbbs gpzch bgfdt vfcppm jkpmsf dlhx nncl rjhs tppntfp thbdr lfspxch xncmd sgslkt gpbzq ghrnz phszhl gjjglc dpclg pvmj gxpmk mgczn nrdz mdtvbb sxpv vpfnsz qgqlv zsfzq vckt kvk kdvrrmmx lfrcfd mtdnt rmvqb sgkmrlt kktsjbh txmjl xpqt rhzbf qjvf xkrm lqrl rgklhlj tkhkf qfbh frtdmqzb jfjr qcrdn dcfj vkxrzx (contains sesame)
hdvkmd dlpvq svkjz glc smspv nrktps ggdbl zgbh pdxcdlq vqh grzznd zfcqk flfsv hnhrv vfcppm zsfzq nbdcsj kbxcj zvd snvzl jlzhsl hfxss shmxg rhzbf xflfvx zrzh ddtkx vsv bmmb tjrrm kktsjbh hbgsgt ldpr nrdz jsrrk rmvqb mndpbq tbxmtm ghrnz phszhl mdtvbb bltrrnl sgkmrlt gdnhx bgrrld xncmd jsjx rgklhlj lnrjbft frpvd cmqs ddkk kdqls dmtfl lfspxch dbxkrn (contains soy, peanuts, fish)
kbj frpvd fbbgv xlzlv zmtj mdtvbb hbgsgt dbxkrn lfzts txmjl jfjr kktsjbh lcggks nmzz zjpzr mtdnt pgvqd hdvkmd bgrrld zfmx pvmj scms hzc pltnp vpfnsz qfbh jm kvjr xkrm kvk tbfxdtm rmvqb ldpr ggjslpt qqfr hjhknn rhzbf dpclg pvhs gbbs kvghnj tzjvdj xflfvx xnrbnl frtdmqzb dlpvq cmqs gpzch bltrrnl zppsxq fsdgvh dsvz pmzcrrd ggdbl qhrd gpbzq mndpbq fpqqbv pdxcdlq mgczn hgn kdqls svkjz txqrj zsfzq pbtq htmkhr zvd lfrcfd qzmnp dzjhj kdvrrmmx jlzhsl xfkhp dcfp mbvpv jsrrk (contains soy)
nrdz zfcqk rhzgdm qcrdn lqrl kdqls phszhl snvzl lthn sgkmrlt kxjt scms pgvqd glc gcp smspv sxpv qzmnp vfcppm sgskhk zrzh jgbmk lnrjbft vpfnsz vmj dqcpjp dcfj jsrrk zgbh dmtfl hkrnqnlt xlzlv zfmx txmjl pnhd mbvpv zsfzq mdtvbb fbbgv frpvd hgn bgfdt gxpmk rnllc qjvf vqh shpglbr kktsjbh dzjhj hzc qfbh qqfr rgklhlj hdvkmd gpn nvhnjb thbdr ddtkx kvjr hnhrv gdzkp vctgp ddkk drkgc jsjx mgczn tbxmtm (contains soy, shellfish)
bgfdt vtbx bjqnjv tjrrm kktsjbh xlzlv vkxrzx vtcj dlpvq kdqls rzh ngxmvj zgbh zvd xflfvx hfxss tppntfp bgrrld zh frpvd zjpzr smspv pmzcrrd mqhl dcfj sdkvkg lfrcfd flfsv mqbv nncl vsxcjpp zsfzq shmxg mcjbn sxpv dcfp grzznd rhzgdm ggdbl jsjx dlhx nvhnjb dbxkrn mtdnt qzmnp jsrrk jgbmk qdmr ldpr dpclg gdnhx mdtvbb dzptlj mlbp nnbn pltnp jm jkpmsf mgczn qjvf tzjvdj rrqtp rnllc zppsxq qtgks thbdr qzczl (contains sesame)
gdzkp pdxcdlq nvvm plhpz qzmnp dzjhj cdr zppsxq dsvz rmvqb qhrd zh hkrnqnlt xknmp snns lfrcfd gpbzq pstspq kvk nvhnjb hnhrv pnhd xlzlv ldpr gcp dzptlj zsfzq gxpmk kdqls lxtzr rhzgdm nmzz tf mgczn lfzts dmtfl cmqs kbbmsc smspv lfspxch dqcpjp kxjt xxltvc kktsjbh shpglbr frpvd qtgks bgrrld tbfxdtm vfcppm vpfnsz zfcqk ddkk frtdmqzb sgslkt nbdcsj vckt xpqt gpn shmxg vtbx xflfvx mdtvbb zjpzr bjgsll drkgc (contains soy)
fpqqbv dpclg vctgp mcjbn kxjt dbxkrn kdvrrmmx kbxcj jgbmk qdmr mgczn phszhl kktsjbh jlzhsl zvd pltnp dmtfl pstspq sgskhk mvxdcbm bmmb txmjl mxrs pnhd kdqls lcggks frtdmqzb hgn sgkmrlt smspv qgqlv lfspxch rzh dcfp tjrrm zgbh zfcqk xvqbk tppntfp pbtq gcp qzczl qqfr grzznd vfcppm hjhknn xfkhp ggdbl mqhl kbj zfmx xlzlv xxltvc ddkk rhzgdm gjmvxg qzmnp jsjx vtcj tkhkf vmj nvvm mdtvbb scg shmxg bjqnjv qcrdn frpvd pgvqd zh pvmj drkgc (contains soy)
grzznd jsrrk zdl kvk zsfzq ddtkx smspv ggdbl nrdz pfnv gpn scms txqrj gcp tzjvdj kvjr rmvqb gjjglc jm kktsjbh xknmp gxpmk tbxmtm dqcpjp fbbgv xkrm mcjbn shmxg hdvkmd mdtvbb kdqls jkpmsf lsrxkjt gdzkp mqhl bjgsll phszhl dlhx sgkmrlt pvhs tkhkf lthn zstc snvzl qcrdn qzmnp fgfzb lnrjbft vtbx bgbvr pstspq ddkk frtdmqzb bgrrld htmkhr qdmr vckt zh zfmx xncmd frpvd zfcqk hjhknn vsxcjpp tbfxdtm kvghnj pgvqd dlpvq (contains soy, wheat)
fsdgvh nbdcsj lfspxch dzjhj plhpz fgfzb gjmvxg thbdr jfjr ddtkx tkhkf ggdbl rmvqb scg nmzz mdtvbb kdqls qgqlv vkxrzx grzznd zfcqk dlhx frtdmqzb rrqtp zgbh tjrrm rjhs drkgc qzczl phszhl mgczn kktsjbh jm xknmp xfkhp vmj sgkmrlt qtgks xpqt kvk pdxcdlq pnhd mqhl tf hkrnqnlt mbvpv dbxkrn lxtzr kxjt bmmb frpvd snvzl lthn qqfr tbxmtm xflfvx kbxcj gdnhx zmtj cmqs xnrbnl (contains peanuts)
sxpv bmmb qtgks bltrrnl xpgnz mdtvbb qzczl vtcj zppsxq vkxrzx zsfzq zh gcp cnmdp kbbmsc qdmr kdqls qqfr vzsjd kbxcj xlzlv bgfdt hfxss sgkmrlt mgczn mqbv glc rhzgdm fpqqbv dzptlj rzh dlpvq frpvd hkrnqnlt zfmx gbbs qzmnp flfsv sdkvkg zfcqk qhrd kktsjbh qjvf txhs snns xfkhp (contains shellfish, fish, dairy)
kbj kbbmsc dsvz mbvpv qgqlv pbtq nnbn frpvd dqcpjp hkrnqnlt zsfzq gjjglc rmvqb fsdgvh drrzq kvjr mlbp gcp ggdbl kdqls mdtvbb vtcj lsrxkjt lxtzr nmzz tbfxdtm vsv snns phszhl xpqt hjhknn kvghnj htmkhr lfzts sgkmrlt mndpbq hgn mgczn jsrrk txqrj tjlgcb jlzhsl zfcqk smspv txhs gbbs dcfp thbdr bjqnjv bgfdt qzmnp bjgsll jsjx (contains soy, shellfish)
tzjvdj kktsjbh lfspxch bjqnjv frpvd jgbmk jsrrk qtgks vtbx txhs gxpmk zfcqk zrzh zjpzr gjmvxg xfkhp bmmb mvxdcbm rgklhlj ggdbl kdqls mgczn flfsv sdkvkg sxpv vmj mcjbn hgn bgbvr hzc lfzts xknmp nncl svkjz xvqbk kvjr mdtvbb dzptlj scg rmvqb dsvz qjvf tjrrm pmzcrrd mqhl nvvm (contains nuts, dairy)
txqrj nbdcsj jsrrk qdmr vkxrzx jfjr plhpz vqh mdtvbb rrqtp dqcpjp xflfvx jm bgbvr kdqls lthn htmkhr vsxcjpp qtgks zjpzr sdkvkg frpvd mgczn snvzl frtdmqzb gcp zsfzq zfcqk hzc xncmd dcfj zgbh pdxcdlq xpqt lcggks zmtj hkrnqnlt zfmx qzmnp gbbs snns xkrm kdvrrmmx dzjhj tzjvdj ggdbl sgkmrlt nrdz bgrrld ghrnz mcjbn hnhrv ddtkx xvqbk vtbx (contains fish)
xxltvc zfcqk kktsjbh dqcpjp gcp glc lcggks nbdcsj tppntfp trmkt snvzl qfbh xknmp gjmvxg ddtkx zppsxq ngxmvj sdkvkg pdxcdlq pstspq qtgks kbj lnrjbft zmtj qdmr bmmb rhzgdm dzptlj jsrrk bjgsll jgbmk mcjbn vqh vpfnsz kbxcj mlbp hfxss tjrrm qhrd hgn rgklhlj nrktps pmzcrrd mndpbq zfmx zvd cdr bgfdt xkrm frtdmqzb bjqnjv bltrrnl mgczn kxjt zsfzq kdqls xnrbnl tjlgcb sxpv nnbn vzsjd shjvxj frpvd rhzbf dzjhj hdvkmd sgkmrlt nmzz dbxkrn gffbft gdzkp fsdgvh vmj mdtvbb vsxcjpp lxtzr (contains fish, wheat, peanuts)
zsfzq grbmvx pdxcdlq mvxdcbm mndpbq pmzcrrd zjpzr mgczn lfspxch kdqls gpn mdtvbb hbgsgt nncl trmkt mcjbn vsv ldpr drrzq rrqtp vsxcjpp grzznd tbfxdtm qdmr zfmx txqrj xkrm nrdz qhrd gjjglc jlzhsl bltrrnl rjhs zrzh xlzlv cdr dzjhj flfsv ggjslpt kxjt pgvqd gdzkp zdl gpzch shpglbr ddkk vtbx zmtj vpfnsz rzh zvd gdnhx cnmdp zcqrgk kbbmsc hjhknn ghrnz kvjr hdvkmd zppsxq tjrrm zh zfcqk xxltvc vckt rhzgdm pvmj xpgnz svkjz frpvd thbdr gbbs hnhrv htmkhr dlhx pnhd sxpv kktsjbh tf gcp pfnv mxrs lsrxkjt gjmvxg (contains shellfish)
ddtkx gpn mbvpv frpvd gdnhx svkjz grbmvx mdtvbb kktsjbh vsv vckt vsxcjpp zsfzq rrqtp bgbvr sxpv fpqqbv dzptlj tkhkf qzczl txmjl frtdmqzb zfcqk dqcpjp tjlgcb tppntfp ghrnz pdxcdlq xvqbk kdqls zdl dpclg qhrd zh xxltvc gpbzq mtdnt mcjbn thbdr drkgc nvhnjb grzznd hzc sgskhk ggdbl hgn hkrnqnlt jgbmk shpglbr jsrrk bmmb vtcj kvghnj (contains wheat, dairy, nuts)
dlpvq fbbgv lthn gpn thbdr pmzcrrd hdvkmd qhrd kbj sdkvkg kvjr mqhl hgn ggdbl bltrrnl mxrs pbtq tjrrm xknmp zjpzr rnllc tf zmtj rrqtp mvxdcbm qqfr htmkhr zfcqk qjvf zcqrgk pstspq ldpr shmxg xxltvc bgfdt jsrrk gxpmk tbfxdtm txqrj bmmb ddkk mcjbn frpvd bgbvr dqcpjp nncl jkpmsf kdqls pvhs zsfzq sgskhk jlzhsl mdtvbb rhzgdm xflfvx glc nmzz shpglbr pgvqd sgkmrlt kktsjbh xkrm rhzbf tppntfp gbbs gpzch gdzkp zh cnmdp (contains wheat)
vsv xpqt gjjglc grbmvx mqhl qgqlv nmzz txqrj mbvpv zrzh jsjx vmj mgczn shmxg qqfr zcqrgk drkgc mxrs gpn fbbgv mdtvbb jm ggdbl jkpmsf pstspq lfzts kvjr vfcppm sgkmrlt tzjvdj bjgsll bgrrld lcggks kbbmsc zsfzq kktsjbh vkxrzx dzjhj snns dbxkrn pgvqd tf frpvd vtcj zjpzr hjhknn zstc zfcqk kxjt trmkt nncl ggjslpt xkrm gdzkp grzznd qfbh cnmdp fsdgvh bmmb tbxmtm qdmr xfkhp qhrd (contains shellfish, nuts, dairy)
pltnp zfcqk tbfxdtm jsrrk zvd kktsjbh fbbgv pdxcdlq ggdbl zjpzr bmmb snvzl lnrjbft kbxcj mdtvbb pstspq scg scms gpn gpbzq ghrnz xnrbnl kvghnj jsjx dqcpjp grzznd tzjvdj hdvkmd mglzsz sdkvkg nrktps pvmj shpglbr xvqbk zmtj kxjt rhzgdm kbbmsc bltrrnl qtgks mgczn mqhl sxpv qgqlv xxltvc frpvd zsfzq xfkhp xncmd nvhnjb shmxg qzczl flfsv zdl pmzcrrd bgfdt lqrl (contains dairy)
pstspq bgbvr rjhs qgqlv tjrrm plhpz qcrdn ggdbl tjlgcb hjhknn zgbh zfmx qjvf vctgp vzsjd pltnp xkrm gpzch gbbs drrzq vtbx xpqt lqrl mgczn shmxg nnbn gxpmk pvhs qhrd zcqrgk pmzcrrd qtgks qzmnp kbj trmkt xfkhp zsfzq nncl drkgc svkjz qzczl tkhkf htmkhr shpglbr vmj vsxcjpp jsrrk kdvrrmmx nvvm ghrnz mxrs kdqls mqhl bmmb xncmd vkxrzx fpqqbv gcp lthn tbxmtm hnhrv sgslkt hbgsgt lxtzr kktsjbh gjmvxg rnllc jm dpclg gffbft tf zmtj frpvd ldpr smspv tppntfp mdtvbb lfzts hgn dzptlj vpfnsz lcggks qfbh kvk (contains nuts)
mvxdcbm dzptlj gxpmk pltnp scg nvvm sdkvkg lnrjbft zdl frpvd shmxg vqh bmmb qcrdn pmzcrrd zrzh kktsjbh mxrs pvhs xxltvc qgqlv ghrnz jlzhsl gpn smspv zfcqk ggdbl zsfzq hkrnqnlt rmvqb gdzkp gpzch lcggks vctgp pdxcdlq kdqls vsxcjpp xncmd txqrj dqcpjp xflfvx mgczn rnllc gjmvxg zjpzr cnmdp vfcppm gcp mqhl nncl bltrrnl ddkk dlpvq zppsxq tbfxdtm fgfzb gpbzq (contains sesame, peanuts, soy)
tbxmtm jkpmsf pvhs jlzhsl xpqt kvghnj xlzlv kxjt zrzh qjvf rmvqb pnhd grzznd txhs scms jsjx mdtvbb shmxg phszhl tkhkf ggjslpt lfspxch trmkt zmtj zsfzq kdqls mxrs qhrd dsvz nvhnjb mbvpv mgczn dmtfl qqfr jsrrk vzsjd pbtq htmkhr hjhknn snvzl scg xvqbk glc bltrrnl mvxdcbm rgklhlj ggdbl pmzcrrd xkrm lxtzr tjrrm sdkvkg ngxmvj bjgsll nnbn plhpz hnhrv qfbh hkrnqnlt pvmj txmjl hfxss fsdgvh frpvd nncl nbdcsj jm gpbzq xflfvx ddkk svkjz hzc lfrcfd kktsjbh hdvkmd tf (contains wheat, nuts, peanuts)
ddkk qzczl fpqqbv dcfp zgbh qzmnp mqbv kdqls gpbzq qtgks mdtvbb kktsjbh fbbgv zrzh kvk cmqs bltrrnl lthn zsfzq shpglbr xfkhp smspv dmtfl mglzsz mbvpv nnbn zppsxq zfcqk jfjr gbbs zh ngxmvj cnmdp pltnp bmmb mgczn mvxdcbm drkgc qhrd zmtj qgqlv nvhnjb nrdz mndpbq vmj thbdr mtdnt xpqt nmzz frpvd gcp rhzgdm vsxcjpp (contains peanuts, dairy, shellfish)
qtgks xlzlv zrzh xflfvx vtbx mxrs gpn gbbs jsrrk xxltvc qgqlv jfjr svkjz sgskhk tjrrm fbbgv bgbvr zsfzq shjvxj tppntfp gdzkp ghrnz sxpv nvvm vtcj dmtfl mdtvbb mgczn dqcpjp rjhs kbxcj vckt pnhd jkpmsf dcfj dbxkrn dzjhj frpvd pvhs ddkk scg mndpbq mcjbn dsvz vzsjd qfbh vfcppm shpglbr cmqs grzznd flfsv htmkhr vctgp ggdbl nrktps dpclg zfcqk txqrj qjvf lqrl pgvqd tbxmtm kktsjbh tf kdvrrmmx cdr nncl (contains shellfish)
jlzhsl vfcppm nrktps lsrxkjt gjjglc kdqls sgslkt rjhs zsfzq rhzbf gpzch xvqbk scg frpvd bgrrld dsvz gcp xlzlv zjpzr qgqlv mcjbn drkgc hzc kktsjbh mgczn zfcqk qhrd xfkhp ggdbl tbxmtm lnrjbft thbdr nvhnjb vzsjd dcfj zh kdvrrmmx zfmx fbbgv pstspq pdxcdlq ddtkx pfnv (contains peanuts, dairy)
";
    }
}
