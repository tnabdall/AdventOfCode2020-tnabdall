﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Day14
{
    class Program
    {
        static void Main(string[] args)
        {
            Part1(PROBLEM_INPUT);
            Part2(PROBLEM_INPUT);
            Console.ReadLine();
        }

        private static void Part2(string input)
        {
            var groups = ParseGroups(input).ToList();
            Dictionary<long, long> memory = new Dictionary<long, long>();
            foreach (var group in groups)
            {
                group.ApplyInstructionsPart2(memory);
            }
            Console.WriteLine(memory.Values.Sum(e => e));
        }

        private static void Part1(string input)
        {
            var groups = ParseGroups(input).ToList();
            Dictionary<long, long> memory = new Dictionary<long, long>();
            foreach(var group in groups)
            {
                group.ApplyInstructionsPart1(memory);
            }
            Console.WriteLine(memory.Values.Sum(e => e));
        }

        private static IEnumerable<MaskInstructionGroup> ParseGroups(string input)
        {
            var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            Mask mask = null;
            List<MemoryInstruction> instructions = new List<MemoryInstruction>();
            foreach(var line in lines)
            {
                if (line.StartsWith("mask"))
                {
                    if (instructions.Count > 0)
                    {
                        yield return new MaskInstructionGroup(mask, instructions);
                        instructions = new List<MemoryInstruction>();
                    }
                    mask = new Mask(line);
                }
                else if (line.StartsWith("mem"))
                {
                    instructions.Add(new MemoryInstruction(line));
                }
                else
                {
                    throw new Exception();
                }
            }
            if (instructions.Count > 0)
            {
                yield return new MaskInstructionGroup(mask, instructions);
            }
        }

        class MaskInstructionGroup
        {
            public Mask Mask { get; }
            public List<MemoryInstruction> Instructions { get; }

            public MaskInstructionGroup(Mask mask, List<MemoryInstruction> instructions)
            {
                Mask = mask ?? throw new ArgumentNullException(nameof(mask));
                Instructions = instructions ?? throw new ArgumentNullException(nameof(instructions));
            }

            public void ApplyInstructionsPart1(Dictionary<long, long> memory)
            {
                foreach(var instruction in Instructions)
                {
                    var instructionVal = ConvertValueToBinaryArray(instruction.Value);
                    if (instructionVal.Length != Mask.MaskArray.Length)
                        throw new Exception("Should be same length");
                    for (int i = 0; i < instructionVal.Length; i++)
                    {
                        instructionVal[i] = Mask.MaskArray[i] ?? instructionVal[i];
                    }
                    memory[instruction.Location] = ConvertBinaryArrayToValue(instructionVal);
                }                
            }

            public void ApplyInstructionsPart2(Dictionary<long, long> memory)
            {
                foreach (var instruction in Instructions)
                {
                    var locationVal = ConvertValueToBinaryArray(instruction.Location);
                    if (locationVal.Length != Mask.MaskArray.Length)
                        throw new Exception("Should be same length");
                    var maskCombos = Mask.GetAllPossibleMaskCombinations();
                    foreach (var mask in Mask.GetAllPossibleMaskCombinations())
                    {
                        var locationValCopy = locationVal.ToArray();
                        for (int i = 0; i < locationValCopy.Length; i++)
                        {
                            locationValCopy[i] = mask[i] ?? locationValCopy[i];
                        }
                        memory[ConvertBinaryArrayToValue(locationValCopy)] = instruction.Value;
                    }
                }
            }

            private int[] ConvertValueToBinaryArray(long value)
            {
                var binaryString = Convert.ToString(value, 2).PadLeft(36, '0');
                return binaryString.Select(e => int.Parse(e.ToString())).Reverse().ToArray();
            }
            private long ConvertBinaryArrayToValue(int[] binaryArray)
            {
                long acc = 0;
                for (int i = 0; i < binaryArray.Length; i++)
                {
                    if (binaryArray[i] == 1)
                        acc += (long)Math.Pow(2, i);
                }
                return acc;
            }
        }

        class Mask
        {
            public int?[] MaskArray;

            public Mask(string input)
            {
                MaskArray = input
                    .Split('=')[1]
                    .Trim()
                    .Select(e =>
                    {
                        switch (e)
                        {
                            case 'X':
                                return null;
                            case '1':
                                return (int?)1;
                            case '0':
                                return (int?)0;
                            default:
                                throw new Exception();
                        }
                    }).Reverse().ToArray();
            }

            public IEnumerable<int?[]> GetAllPossibleMaskCombinations()
            {
                var nullCount = MaskArray.Count(e => e == null);
                if (nullCount == 0)
                {
                    yield return MaskArray;
                    yield break;
                }
                else
                {
                    var nullIndices = new List<int>();
                    for (int i = 0; i < MaskArray.Length; i++)
                    {
                        if (MaskArray[i] == null)
                            nullIndices.Add(i);
                    }

                    // Get all possible combinations
                    // where its included we give it value 1, where its not we give it value 0
                    for (int k = 1; k <= nullIndices.Count; k++)
                    {
                        var combs = GetKCombs(nullIndices, k);
                        foreach (var comb in combs)
                        {
                            int?[] newMaskArray = CreateMaskArrayWith1sAtSelectedIndices(nullIndices, comb);
                            yield return newMaskArray;
                        }
                    }

                    // Include the all 0s case
                    yield return CreateMaskArrayWith1sAtSelectedIndices(nullIndices, new int[] { });
                }

                int?[] CreateMaskArrayWith1sAtSelectedIndices(List<int> allIndices, IEnumerable<int> _1Indices)
                {
                    // Create new array (hard copy)
                    // Also, convert all 0s to nulls here (as they now take on the unchanged behaviour)
                    int?[] newMaskArray = MaskArray.Select(e => e == 1 ? (int?)e.Value : null).ToArray();
                    foreach (var index in _1Indices)
                    {
                        newMaskArray[index] = 1;
                    }
                    foreach (var index in allIndices.Except(_1Indices))
                    {
                        newMaskArray[index] = 0;
                    }
                    
                    return newMaskArray;
                }
            }

            static IEnumerable<IEnumerable<T>> GetKCombs<T>(IEnumerable<T> list, int length) where T : IComparable
            {
                if (length == 1) return list.Select(t => new T[] { t });
                return GetKCombs(list, length - 1)
                    .SelectMany(t => list.Where(o => o.CompareTo(t.Last()) > 0),
                        (t1, t2) => t1.Concat(new T[] { t2 }));
            }
        }

        class MemoryInstruction
        {
            public long Location { get; set; }
            public long Value { get; set; }

            public MemoryInstruction(string input)
            {
                Location = long.Parse(input.Split(']')[0].Split('[')[1]);
                Value = long.Parse(input.Split('=', StringSplitOptions.RemoveEmptyEntries)[1]);
            }
        }

        static void AddOrCreate<T>(Dictionary<T, long> dict, T key, long value)
        {
            if (!dict.ContainsKey(key))
                dict[key] = value;
            else
                dict[key] += value;
        }

        const string EXAMPLE_INPUT = @"mask = XXXXXXXXXXXXXXXXXXXXXXXXXXXXX1XXXX0X
mem[8] = 11
mem[7] = 101
mem[8] = 0";

        const string EXAMPLE_INPUT_2 = @"mask = 000000000000000000000000000000X1001X
mem[42] = 100
mask = 00000000000000000000000000000000X0XX
mem[26] = 1";

        const string PROBLEM_INPUT = @"mask = 11100XX0000X1101X1010100X1010001XX0X
mem[24196] = 465592
mem[17683] = 909049
mem[28999] = 20912603
mem[22864] = 7675
mem[55357] = 6401
mem[47006] = 1087112
mask = 111X000100XX1X01X1X10X01X11101100010
mem[22535] = 42768
mem[3804] = 1432484
mem[5475] = 5972
mem[24585] = 484096364
mem[56009] = 206637948
mem[30917] = 630
mem[28325] = 1467510
mask = 1110X0X0001111011101101011X01000X10X
mem[432] = 23010545
mem[31135] = 250
mem[60949] = 3366
mask = X10101X10X0X100111X11001X0001X11X011
mem[19601] = 169998617
mem[25958] = 295048
mem[277] = 58248
mem[47301] = 2234
mask = 1X101110000001X1101X101X000010101101
mem[27397] = 328720
mem[23718] = 91033
mem[42581] = 1336
mem[39656] = 58221
mask = 101000100011110111X110111011XXX00000
mem[55993] = 21914
mem[8278] = 287934896
mem[7186] = 199312871
mem[35770] = 128786
mem[13001] = 1187
mem[5951] = 34516681
mask = 1110000000101X0X1101100000X11X010001
mem[51573] = 6865197
mem[12594] = 57
mem[43156] = 1988185
mem[194] = 125226832
mem[11055] = 1304
mem[8009] = 15347
mem[39550] = 434
mask = 0100X10X0100X000101001X10X1101001111
mem[4865] = 6399850
mem[54408] = 1063909
mem[37625] = 391570382
mem[15539] = 3309
mask = 1X1000XX0XX01X00110X1101110X0X001110
mem[8011] = 39113
mem[4279] = 54926
mem[24196] = 66470405
mask = 10X0000000X0110X1X01X1X0X001111X1011
mem[9092] = 362810946
mem[47138] = 16850639
mem[41856] = 18821263
mask = 1110000X00X01101X1010XX101010X0XX0X1
mem[50504] = 27298
mem[57026] = 107914
mem[52684] = 127977723
mem[12905] = 3044843
mem[23942] = 34437
mem[18697] = 94002
mask = 1110X010100X11X1110111011101110X1XX0
mem[50504] = 28170
mem[56861] = 48063
mask = 1110101000X001X110110010000X0110XX01
mem[31344] = 15906303
mem[49101] = 18755
mem[50877] = 5126106
mem[27011] = 29112904
mem[11552] = 184372544
mem[27085] = 1711926
mem[57734] = 11
mask = 01X10X000000110110011010X10X000010X0
mem[57703] = 450
mem[55911] = 897776
mem[39593] = 9365920
mem[27918] = 111787
mask = 11100010X0X1110X0X000010X0X00000110X
mem[21285] = 142940
mem[48827] = 1348
mem[47138] = 791004800
mem[17054] = 170726381
mask = 1X1X00X000XX110111010X10XX101X001001
mem[33192] = 5106641
mem[44523] = 205714
mem[2397] = 481
mem[55624] = 3188
mem[26269] = 3891595
mem[14130] = 17865
mem[45278] = 995854
mask = 0101010100011XX1111101000X0X11110000
mem[54355] = 118
mem[37759] = 26260
mem[17076] = 89114435
mem[194] = 1039
mask = 11100001011X1X0X110X100XX1X00000X111
mem[56508] = 5551
mem[24388] = 8654
mask = 1111001010101101110111X10XX0100X1100
mem[54164] = 209
mem[51296] = 377456
mem[17932] = 27062366
mem[6263] = 5509
mask = 00X1010X00001X0X11X101001011X0000000
mem[19882] = 8803576
mem[53384] = 3337517
mem[56996] = 28569
mem[45196] = 176
mem[35228] = 451184
mem[28999] = 1358694
mem[11073] = 106613
mask = 0XX101010X001101110100001XX1101X0000
mem[18074] = 29316
mem[18560] = 6691478
mem[11628] = 29149
mem[17246] = 4374
mem[7371] = 596
mask = X10X01XX00001101X101X10010011X1000XX
mem[20505] = 833
mem[51543] = 514
mem[33874] = 1486233
mask = 01X10101010000011111000101100XXXX111
mem[41142] = 8151
mem[57816] = 850
mem[3408] = 25361551
mem[49742] = 84
mem[533] = 4697
mask = 1100X010X100XXX011010X00XX01000X1010
mem[18925] = 6495
mem[51352] = 2712
mem[35009] = 237034
mem[46199] = 6606255
mem[58066] = 10080453
mem[248] = 176262
mask = 111XX0X0X0X011X11X010001000100X11100
mem[56816] = 26145436
mem[6953] = 387283
mem[50052] = 84
mem[56530] = 141676
mem[60168] = 10908459
mem[17685] = 287
mask = 111000010X1011011X010111X1110111001X
mem[52094] = 1398677
mem[36982] = 4553
mem[24348] = 1001783
mem[28987] = 11213376
mem[14039] = 7011973
mem[52684] = 287185
mask = 11110010X01011XX1XX110X1001110111100
mem[17212] = 36655
mem[24740] = 10507
mem[7497] = 2603
mask = 11X0000100001XX10101X011X0100XX01100
mem[8722] = 1860757
mem[4361] = 9089027
mem[43286] = 764265
mask = 111000X00X00110111110X000101000X0101
mem[21] = 10308359
mem[55077] = 1923265
mask = 11100X001010110111010XX1101X00011X01
mem[51543] = 272
mem[46636] = 41481200
mem[11331] = 16116
mem[4157] = 24217194
mem[39683] = 36276
mem[47837] = 75704
mask = 001XX10100X0011111010X00X11000011010
mem[64404] = 3069125
mem[432] = 42797141
mem[11073] = 655
mem[17402] = 687682
mask = X1111X100000111X100X11X0110X11XX1110
mem[5138] = 3437645
mem[64302] = 10748
mem[22052] = 7646851
mem[57307] = 1846
mem[8556] = 2513
mem[20862] = 2137
mem[47082] = 65210668
mask = 111000X0XX011001011X01110X0X111X0000
mem[24216] = 140969206
mem[64406] = 1006
mem[55744] = 282152566
mem[26368] = 8935044
mem[9525] = 581
mask = X10XXX010X0X00001110101111X0X0010001
mem[37409] = 830
mem[55256] = 1293327
mem[49887] = 10300024
mask = 11100010000111010X0X0X10XX1X1X010010
mem[7371] = 10089034
mem[55184] = 992391
mem[8128] = 2573
mem[16780] = 12198200
mask = X110010X00001101110X1X0X1001X01X1X00
mem[17942] = 3204736
mem[43265] = 160130
mem[54791] = 905
mem[43494] = 31105546
mem[23836] = 5693
mask = 01X10001X100000001000111111X0101X010
mem[52752] = 261536
mem[33121] = 637759448
mem[38648] = 1999
mem[10484] = 4506
mask = X1100001000XX101X1X11010001X1X00X00X
mem[11033] = 303386685
mem[52968] = 76
mem[692] = 80215
mem[32901] = 410
mem[1170] = 1019569
mask = 0100010X000011XX010101001XX0X111X110
mem[33121] = 1004697269
mem[40768] = 9652471
mem[46911] = 51740
mask = 0111010X000011XX100101000100011X1011
mem[31501] = 127217457
mem[8760] = 1331
mem[53277] = 100190787
mem[11055] = 816674979
mem[20322] = 5235075
mask = X1X10X0X000011011X01011X11X1000X110X
mem[34070] = 743
mem[7184] = 550637
mem[19585] = 921346
mem[9587] = 217676716
mask = 111010100000X101X0110000XX1000X10000
mem[28281] = 77
mem[38263] = 1881
mem[28451] = 444240911
mem[43156] = 25309
mem[30888] = 660
mem[2469] = 454
mask = 11X00X100X00110X110100X00001101X01X1
mem[22535] = 480526992
mem[64908] = 1012
mem[18554] = 30457232
mem[7497] = 896738
mem[21289] = 56826187
mem[51296] = 467
mask = 1110001X00X111X1X101010X01010101X001
mem[55693] = 18382
mem[27601] = 41915
mem[22616] = 17257
mem[19492] = 4667
mem[6953] = 40034760
mask = 1X11000000X0X10X1X0X0010110XX1001110
mem[36049] = 431
mem[53464] = 3399
mem[47435] = 3768554
mem[13326] = 10597
mask = 1X10000000011001011001X10000110X01X1
mem[116] = 17770
mem[23833] = 174842
mem[58867] = 3425856
mem[45278] = 118643
mem[44440] = 584704
mask = X1100001000X1X010X011X0X0000XX0X1001
mem[40768] = 85230060
mem[36744] = 690218
mem[23942] = 369155
mem[46690] = 879641338
mem[62388] = 36434630
mem[45788] = 1712
mask = X110X011001111010X010100010000110X01
mem[20289] = 8051
mem[5798] = 64870413
mem[20409] = 12365
mask = 1X1X000X001111XX110100X1001X00X00011
mem[52329] = 9963755
mem[44763] = 911974581
mem[38281] = 1670
mem[30121] = 12169074
mem[60457] = 1795732
mem[59755] = 4148431
mask = 010X0X010100X00X1X1X1X1101101X0X0000
mem[65073] = 19189703
mem[21343] = 199170
mem[31147] = 237218
mem[63870] = 119469
mem[45728] = 7227071
mem[52235] = 232889
mask = 11X0001000011XX101X0001110X110000000
mem[51221] = 414
mem[51296] = 97635
mem[44882] = 597
mem[21580] = 151
mem[62354] = 15729
mem[49238] = 154335
mem[8722] = 6568
mask = 111001010000X10X11001011X00010110X01
mem[10789] = 11719
mem[44455] = 5005
mem[39994] = 1548
mem[2472] = 182989
mem[30152] = 6772910
mem[13360] = 23778
mask = 01X0000101X0X00111110011100X111010X0
mem[40929] = 2617123
mem[24670] = 21298246
mem[19730] = 53122
mem[4453] = 16356635
mem[18167] = 152861
mem[6794] = 109114900
mask = X1010111000X100111X1XXX111X10X110110
mem[60242] = 527195070
mem[9092] = 14828
mem[60561] = 12393
mask = 11X000000000110X110100X0110X01011000
mem[43733] = 3381891
mem[28994] = 102996
mem[34677] = 742154643
mem[56117] = 51070
mem[62864] = 513
mask = 1110XX100000X1X1X0110X0X0X0X10100001
mem[20020] = 102468
mem[54641] = 518248411
mem[42588] = 133137430
mem[49903] = 393777077
mask = 1X100X010XX011011101000X00XXX100011X
mem[59041] = 130847732
mem[55184] = 745
mem[35978] = 1728
mask = 1110101000X011011011101000X110X1X001
mem[32661] = 302743
mem[4437] = 640
mem[8811] = 1334
mem[4453] = 4515
mem[39550] = 598022
mask = 1110X100X0X111010XX11100111110000010
mem[4036] = 120228
mem[35400] = 285627
mem[13330] = 484890
mem[42246] = 9741695
mask = X111X0000010X1001101X100110100000001
mem[4865] = 424
mem[14807] = 2916
mem[9469] = 470087
mem[10170] = 238048
mask = 1110001000X0X101111X110001010010100X
mem[54355] = 114424821
mem[51406] = 899
mem[1515] = 391
mem[38678] = 103708
mem[53982] = 121098
mem[49442] = 9789
mem[35604] = 19583721
mask = 111X000000001101X10111X01101100X1101
mem[43682] = 175571639
mem[3831] = 9455072
mem[44523] = 106569
mem[37257] = 315345859
mem[64406] = 1612
mask = 01XX0001X00X1101X101000X010000001111
mem[19343] = 31345288
mem[8756] = 1677
mem[29688] = 81
mem[52684] = 332229
mem[15099] = 3806446
mem[38338] = 11840
mem[55633] = 116815478
mask = 1110001X00011X01X00X010110X011000010
mem[27344] = 2606
mem[5054] = 810
mask = 1101010X00001X0111010101100X1X10X010
mem[21289] = 4994
mem[16780] = 851612834
mem[18737] = 50882
mem[17717] = 2670
mem[36049] = 578215133
mem[47082] = 496908
mask = X0X101X1X00001111101X1X10111X0X1101X
mem[41026] = 409998
mem[14596] = 704
mem[29771] = 3494643
mem[25660] = 113256355
mem[59154] = 2369
mem[62847] = 2904
mask = 1110000000X111X1110110X011X1000X100X
mem[5769] = 132360
mem[42087] = 3851681
mem[40768] = 1010
mask = 1X100X0X000011011101X10X0100X00000X1
mem[33025] = 1410632
mem[43181] = 2000
mem[24043] = 700
mask = 111000X100001101110100X0X1XX01X00X11
mem[33195] = 1151261
mem[28725] = 6917
mem[21285] = 418795
mask = 1X110X00001X110111010X1XXX0100110X01
mem[1848] = 1543602
mem[20484] = 52183944
mem[24869] = 1750
mem[28451] = 146868
mem[31319] = 3136
mem[49411] = 3378
mem[45914] = 97061
mask = 1X10X000001111X1X1011011111X001X1011
mem[12905] = 145078660
mem[52038] = 2552
mem[34070] = 24245910
mem[11552] = 26334460
mem[769] = 22524
mem[45411] = 162871607
mask = 01110XX101X01X0111X1X0X11100101100X0
mem[34492] = 43083
mem[5855] = 258360838
mem[6147] = 282
mem[63064] = 5211483
mask = 1X10000000111X00110X0X110X1000000011
mem[9866] = 3514
mem[54099] = 14778
mask = X1X1000X0X1X11001101001101000101X0XX
mem[9299] = 24521049
mem[61776] = 292226032
mem[18074] = 19038893
mem[31659] = 114327725
mask = 010X010100001001X101100X0XX011X00X1X
mem[47192] = 499
mem[35562] = 1127803
mem[27862] = 7106662
mask = 1XX000X100101101110X1010010X00001X01
mem[8438] = 44700
mem[45696] = 470154968
mem[41741] = 535
mem[51296] = 550
mem[11976] = 153002786
mem[45322] = 84583483
mem[29651] = 15576
mask = 11100010000011011XX1010XXX010X01X000
mem[1789] = 911
mem[56530] = 115914529
mem[5451] = 142
mem[277] = 933970424
mem[64908] = 5581695
mem[55077] = 199
mask = 1010001X00X111011XX11001X0001000X101
mem[23536] = 53655494
mem[14360] = 754122
mem[15890] = 36708
mem[44816] = 99834802
mem[20566] = 473274604
mask = 111X101000X00X110X1X100001X011110X00
mem[33169] = 51545746
mem[31147] = 2815
mem[16106] = 1393423
mem[8786] = 9878148
mask = 11100010001111X0X0XX0110XX00X0001111
mem[47869] = 330626
mem[10389] = 1173
mask = 111001000000110111010000010X100XX10X
mem[56037] = 3906806
mem[42588] = 6113
mem[27703] = 795872
mem[52731] = 70918735
mem[21662] = 2974689
mem[38094] = 92515
mem[59061] = 75564
mask = 1XX0000X0X10110111X11X100X0X01100111
mem[17179] = 211
mem[25555] = 111231
mem[4258] = 24945299
mem[24124] = 4414744
mem[11003] = 7486
mem[25891] = 496
mask = X1110000X0X0110110001X11X1X10X000X10
mem[4841] = 236728268
mem[420] = 298
mem[34895] = 412844940
mem[9587] = 6377
mask = 1110XX1000001101100100010X01000111X0
mem[20505] = 717616953
mem[55911] = 254
mem[18500] = 12753143
mem[24196] = 2549282
mem[44072] = 370306692
mask = 111X0000001X110X1101X01XX100000X1XX1
mem[42453] = 5827
mem[6953] = 203298
mem[56264] = 375
mem[34887] = 2940099
mem[65469] = 351297914
mem[62106] = 226948
mask = 010X0X010100X00XX1XX1011111X0110100X
mem[6566] = 13861612
mem[62354] = 397490
mem[15192] = 26357463
mem[61353] = 120260489
mem[48329] = 20209615
mask = X1000X100100110111011X10100XX1100000
mem[31626] = 11216
mem[60050] = 922
mem[13079] = 460592139
mem[22864] = 3460
mem[21808] = 427851033
mem[26759] = 747148
mask = 111000X0X00011XX1001X01000011000X0XX
mem[47077] = 21552651
mem[45076] = 4796
mem[59796] = 3055
mem[61256] = 6500
mem[16825] = 980934769
mask = 1110X01101XX1100110X0000X0000X001100
mem[45076] = 584
mem[54926] = 29718
mem[18560] = 3999638
mem[4865] = 1007977570
mask = 01X10X010100X001111110X1X1XX10110XX1
mem[28725] = 172494865
mem[60439] = 7496408
mem[55184] = 111489
mem[15543] = 8109490
mem[25555] = 203769
mask = 101101X00011110111010X1XX011X0110001
mem[40538] = 2820
mem[12808] = 242352332
mem[55693] = 796801
mask = 11100110000X11X11101111X11110X010100
mem[52638] = 130173
mem[41136] = 99225
mem[30981] = 145765
mem[24654] = 23239
mem[56080] = 2600371
mask = 10110X1X000XXXXX1101001X01111011101X
mem[25555] = 35039981
mem[51117] = 899042
mem[28546] = 27200807
mask = 11100X10X00X11011X1111000X0111XX0001
mem[34877] = 19623
mem[32929] = 212201
mem[50492] = 86
mem[25692] = 3967
mem[59620] = 4489
mem[2397] = 167
mem[1613] = 1929
mask = 111001X00000XX0111010X0101X10001X1XX
mem[33026] = 1738730
mem[53181] = 9011944
mem[33688] = 9432725
mem[28325] = 2913917
mem[4760] = 30224638
mem[44900] = 23686766
mask = X010000X0010X100X10001X0X11111001XX0
mem[62904] = 543471105
mem[39372] = 284
mem[54084] = 99353
mem[959] = 565028
mem[41884] = 23617874
mask = 011100001000X1X110X0XXX1X1X1001X0001
mem[62428] = 82
mem[25406] = 1767
mem[9987] = 27303911
mem[45698] = 21354675
mem[33688] = 140290550
mem[11725] = 2764898
mask = 111000100000XX01110X010XX00101010100
mem[41653] = 490
mem[29936] = 361323
mem[61683] = 6863356
mem[19368] = 540
mem[61836] = 4792848
mem[57816] = 984651521
mem[39494] = 141986806
mask = 1X00101011X0X00X11X11110000000000011
mem[6510] = 25364
mem[45411] = 2282830
mem[24451] = 39824
mem[47536] = 125549431
mask = 11X0XX01000X110101010001001100010010
mem[44288] = 998863
mem[29095] = 926329
mem[38956] = 8054373
mem[12846] = 31513
mem[3837] = 14880
mem[44605] = 354763708";
    }
}
