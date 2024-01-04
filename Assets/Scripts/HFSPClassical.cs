using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data.Common;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

public class HFSPClassical : MonoBehaviour
{
    [SerializeField]
    int maxgen;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public (double[,], int[,], double) Solver(int[,] workTime, int[] equSize, int pieceSize, int procSize, int[,] transTime, int[] Power, double[] h_j, double b_l)
    {
        int model = 3;//通过更改0,1,2,3来改变最后输出的是完工时间还是能耗还是结合
        // int[,] workTime = new int[,] { { 10, 9, 12 }, { 6, 13, 18 }, { 4, 13, 19 }, { 10, 15, 18 }, { 13, 13, 7 }, { 13, 5, 9 }, { 7, 14, 9 } };
        // int[] equSize = new int[] { 2, 3, 2 };
        // int pieceSize = 7;
        // int procSize = 3;
        // int[,] transTime = new int[,] { { 1, 2 }, { 5, 2 }, { 10, 10 }, { 2, 8 }, { 4, 5 }, { 5, 5 }, { 7, 10 } };
        // int[] Power = new int[] { 240, 2800, 279, 1125, 264, 1478, 390, 1904, 369, 1120, 103, 1680, 103, 1680 };
        // double[] h_j = new double[] { 0.1502, 0.2113, 0.1338, 0.0553, 0.1799, 0.1266, 0.1430 };
        // double b_l = 0.5;
        int popSize = 20;
        double cr = 0.4;
        double mr = 0.02;
        double[] bestModel = new double[maxgen];
        int[,] bestPop = new int[maxgen, pieceSize];
        double[] avgModel = new double[maxgen];
        List<double[,]> bestptr = new List<double[,]>();
        List<int[,]> bestper = new List<int[,]>();
        double[,] save_fitness = new double[popSize, maxgen];
        int[,] pop = InitPop(popSize, pieceSize);
        for (int gen = 0; gen < maxgen; gen++)
        {
            double[] blance, PC, eT, objValue;
            List<double[,]> ptr = new List<double[,]>();
            List<int[,]> per = new List<int[,]>();
            List<double[]> Model = new List<double[]>();
            CalObjValue(pop, workTime, equSize, transTime, h_j, Power, b_l, out blance, out PC, out eT, out objValue, ptr, per);
            Model.Add(objValue);
            Model.Add(PC);
            Model.Add(eT);
            Model.Add(blance);
            double temp_bestModel = Model[model].Min();
            int temp_bestIndex = Array.IndexOf(Model[model], temp_bestModel);
            bestModel[gen] = temp_bestModel;
            for (int i = 0; i < pieceSize; i++)
            {
                bestPop[gen, i] = pop[temp_bestIndex, i];
            }
            avgModel[gen] = Model[model].Average();
            bestptr.Add(ptr[temp_bestIndex]);
            bestper.Add(per[temp_bestIndex]);
            double[] fitness = CalFitness(Model[model]);
            for (int i = 0; i < popSize; i++)
            {
                save_fitness[i, gen] = fitness[i];
            }
            pop = Selection(pop, fitness);
            pop = Crossover(pop, cr);
            pop = Mutation(pop, mr);
        }
        int finalIndex = Array.IndexOf(bestModel, bestModel.Min());
        return (bestptr[finalIndex], bestper[finalIndex], bestModel.Min());
    }

    static int[,] Selection(int[,] pop, double[] fitness)
    {
        int popSize = pop.GetLength(0);
        int pieceSize = pop.GetLength(1);
        int[,] spop = new int[popSize, pieceSize];
        double sumfit = fitness.Sum();
        fitness = fitness.Select(x => x / sumfit).ToArray();
        fitness = fitness.Select((x, i) => fitness.Take(i + 1).Sum()).ToArray();
        double[] r = new double[popSize];
        System.Random rand = new System.Random();
        for (int i = 0; i < popSize; i++)
        {
            r[i] = rand.NextDouble();
        }
        Array.Sort(r);
        int j = 0;
        for (int i = 0; i < popSize; i++)
        {
            while (fitness[j] < r[i])
            {
                j++;
            }
            for (int k = 0; k < pieceSize; k++)
            {
                spop[i, k] = pop[j, k];
            }
        }
        int[] rand_spopRow = Enumerable.Range(0, popSize).OrderBy(x => rand.Next()).ToArray();
        int[,] spop_temp = new int[popSize, pieceSize];
        for (int i = 0; i < popSize; i++)
        {
            for (int k = 0; k < pieceSize; k++)
            {
                spop_temp[i, k] = spop[rand_spopRow[i], k];
            }
        }
        return spop_temp;
    }

    static int[,] Mutation(int[,] pop, double mr)
    {
        int popsize = pop.GetLength(0);
        int piecesize = pop.GetLength(1);
        int[,] mpop = new int[popsize, piecesize];
        mpop = pop;
        System.Random rand = new System.Random();
        for (int i = 0; i < popsize; i++)
        {
            if (rand.NextDouble() > mr)
            {
                continue;
            }
            int r1 = rand.Next(piecesize);
            int r2 = rand.Next(piecesize);
            int temp = mpop[i, r1];
            mpop[i, r1] = mpop[i, r2];
            mpop[i, r2] = temp;
        }
        return mpop;
    }

    static int[,] InitPop(int popSize, int dnaLength)
    {
        int[,] pop = new int[popSize, dnaLength];
        System.Random rand = new System.Random();
        for (int i = 0; i < popSize; i++)
        {
            int[] temp = Enumerable.Range(1, dnaLength).OrderBy(x => rand.Next()).ToArray();
            for (int k = 0; k < dnaLength; k++)
            {
                pop[i, k] = temp[k];
            }
        }
        return pop;
    }

    static int[] GenRandLU(int maxnum)
    {
        System.Random rand = new System.Random();
        int r1 = rand.Next(maxnum);
        int r2 = rand.Next(maxnum);
        while (r2 == r1)
        {
            r2 = rand.Next(maxnum);
        }
        int rl = Math.Min(r1, r2);
        int ru = Math.Max(r1, r2);
        return new int[] { rl, ru };
    }

    static int[,] Crossover(int[,] pop, double cr)
    {
        int popSize = pop.GetLength(0);
        int pieceSize = pop.GetLength(1);
        int[,] cpop = pop;
        int crossSize = popSize % 2 == 0 ? popSize : popSize - 1;
        System.Random rand = new System.Random();
        for (int crossIndex = 0; crossIndex < crossSize; crossIndex += 2)
        {
            if (rand.NextDouble() > cr)
            {
                continue;
            }
            int[] rlru = GenRandLU(pieceSize);
            int[] father = GetRow(pop, crossIndex);
            int[] mother = GetRow(pop, crossIndex+1);
            if (father.SequenceEqual(mother))
            {
                continue;
            }
            int[] son = new int[pieceSize];
            int[] daughter = new int[pieceSize];
            Array.Copy(mother, rlru[0], son, rlru[0], rlru[1] - rlru[0] + 1);
            Array.Copy(father, rlru[0], daughter, rlru[0], rlru[1] - rlru[0] + 1);
            int j = 0;
            for (int k = 0; k < pieceSize; k++)
            {
                if (k >= rlru[0] && k <= rlru[1])
                {
                    continue;
                }
                while (j < father.Length && son.Contains(father[j]))
                {
                    j++;
                }
                if (j == father.Length)
                {
                    break;
                }
                    son[k] = father[j];
            }
            j = 0;
            for (int k = 0; k < pieceSize; k++)
            {
                if (k >= rlru[0] && k <= rlru[1])
                {
                    continue;
                }
                while (j < mother.Length && daughter.Contains(mother[j]))
                {
                    j++;
                }
                if (j == mother.Length)
                {
                    break;
                }
                daughter[k] = mother[j];
            }
            SetRow(cpop, crossIndex, son);
            SetRow(cpop, crossIndex + 1, daughter);
        }
        return cpop;
    }

    static void CalObjValue(int[,] pop, int[,] workTime, int[] equSize, int[,] transTime, double[] h_j, int[] Power, double b_l, out double[] blance, out double[] PC, out double[] eT, out double[] objValue, List<double[,]> ptr, List<int[,]> per)
    {
        int popSize = pop.GetLength(0);
        int pieceSize = pop.GetLength(1);
        int procSize = equSize.Length;
        objValue = new double[popSize];
        blance = new double[popSize];
        PC = new double[popSize];
        eT = new double[popSize];
        for (int pop_item = 0; pop_item < popSize; pop_item++)
        {
            double Power_Consumption = 0;
            int[] pieceDNA = GetRow(pop, pop_item);
            double[] equStateSeq = new double[equSize.Sum()];
            int[] procSep = CumSum(equSize);
            double[,] pieceTimeRecord = new double[pieceSize, procSize * 3];
            int[,] pieceEquRecord = new int[pieceSize, procSize];
            double[,] Table = new double[equSize.Sum(), (pieceSize + 2) * 3];
            int[] count = new int[equSize.Sum()];
            for (int pro = 0; pro < procSize; pro++)
            {
                int[] pieceList;
                if (pro == 0)
                {
                    pieceList = Enumerable.Range(0, pieceSize).OrderBy(x => pieceDNA[x]).ToArray();
                }
                else
                {
                    double[] tempStopTime = GetColumn(pieceTimeRecord, (pro - 1) * 3 + 1);
                    double[] tempEndTime = GetColumn(pieceTimeRecord, (pro - 1) * 3 + 2);
                    pieceList = Enumerable.Range(0, pieceSize).OrderBy(x => tempEndTime[x]).ToArray();
                }
                for (int pieceIndex = 0; pieceIndex < pieceList.Length; pieceIndex++)
                {
                    int piece = pieceList[pieceIndex];
                    double[] equTimeList = equStateSeq.Skip(procSep[pro] - equSize[pro]).Take(equSize[pro]).ToArray();
                    int[] equList = Enumerable.Range(0, equSize[pro]).OrderBy(x => equTimeList[x]).ToArray();
                    int equ = equList[0];
                    double pieceStartTime = pro == 0 ? 0 : pieceTimeRecord[piece, pro * 3 - 1];
                    double startTime = Math.Max(equTimeList.Min(), pieceStartTime);
                    double stopTime = startTime + workTime[piece, pro];
                    int equStateSeqIndex = procSep[pro] - equSize[pro] + equ;
                    pieceTimeRecord[piece, pro * 3 + 0] = startTime;
                    pieceTimeRecord[piece, pro * 3 + 1] = stopTime;
                    double endTime = pro < 2 ? stopTime + transTime[piece, pro] : stopTime;
                    equStateSeq[equStateSeqIndex] = endTime;
                    pieceTimeRecord[piece, pro * 3 + 2] = endTime;
                    pieceEquRecord[piece, pro] = equ;
                    for (int i = 0; i < 3; i++)
                    {
                        Table[equStateSeqIndex, count[equStateSeqIndex]] = pieceTimeRecord[piece, pro * 3 + i];
                        count[equStateSeqIndex]++;
                    }
                }
            }
            // 获取当前种群下总加权完工时间，也即目标函数值
            objValue[pop_item] = h_j.Zip(GetColumn(pieceTimeRecord, pieceTimeRecord.GetLength(1) - 1), (h, time) => h * time).Sum();
            // 获取当前种群下完工时间
            eT[pop_item] = pieceTimeRecord.Cast<double>().Max();
            // 累加每个工件加工能耗和运输时机器空载能耗
            for (int i = 0; i < equSize.Sum(); i++)
            {
                int j = 0;
                while (Table[i, j * 3 + 2] - Table[i, j * 3 + 3] <= 0)
                {
                    Power_Consumption += (Table[i, j * 3 + 1] - Table[i, j * 3]) * (Power[i * 2 + 1] - Power[i * 2]);
                    j++;
                }
                Power_Consumption += (Table[i, j * 3 + 1] - Table[i, j * 3]) * (Power[i * 2 + 1] - Power[i * 2]);
                Power_Consumption += (GetRowD(Table, i).Max() - GetRowD(Table, i)[0]) * Power[i * 2];
            }
            // 获取当前种群下总能耗
            PC[pop_item] = Power_Consumption;
            //获取当前代次种群下加权能耗和完工时间
            blance[pop_item] = b_l * PC[pop_item] / 4000 + (1 - b_l) * eT[pop_item];
            ptr.Add(pieceTimeRecord);
            per.Add(pieceEquRecord);
        }
    }

    static double[] CalFitness(double[] objValue)
    {
        double[] fitness = new double[objValue.Length];
        for (int i = 0; i < objValue.Length; i++)
        {
            fitness[i] = 1 / objValue[i];
        }
        return fitness;
    }

    

    static int[] GetRow(int[,] pop, int rowIndex)
    {
        int pieceSize = pop.GetLength(1);
        int[] row = new int[pieceSize];
        for (int i = 0; i < pieceSize; i++)
        {
            row[i] = pop[rowIndex, i];
        }
        return row;
    }

    static double[] GetRowD(double[,] pop, int rowIndex)
    {
        int pieceSize = pop.GetLength(1);
        double[] row = new double[pieceSize];
        for (int i = 0; i < pieceSize; i++)
        {
            row[i] = pop[rowIndex, i];
        }
        return row;
    }

    static double[] GetColumn(double[,] pop, int columnIndex)
    {
        int pieceSize = pop.GetLength(0);
        double[] column = new double[pieceSize];
        for (int i = 0; i < pieceSize; i++)
        {
            column[i] = pop[i, columnIndex];
        }
        return column;
    }

    static void SetRow(int[,] pop, int rowIndex, int[] row)
    {
        int pieceSize = pop.GetLength(1);
        for (int i = 0; i < pieceSize; i++)
        {
            pop[rowIndex, i] = row[i];
        }
    }

    static int[] CumSum(int[] input)
    {
        int[] output = new int[input.Length];
        int sum = 0;
        for (int i = 0; i < input.Length; i++)
        {
            sum += input[i];
            output[i] = sum;
        }
        return output;
    }
}
