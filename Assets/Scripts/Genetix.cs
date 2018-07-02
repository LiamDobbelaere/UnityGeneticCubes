using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;
using System;

public class Genetix : MonoBehaviour {
    enum GenetixPhase
    {
        Starting,
        InitializePopulation,
        EvaluateFitness,
        SCM
    }

    public class Individual : IComparable<Individual>
    {
        public float Fitness { get; set; }
        public bool[] Chromosome { get; set; }
        public bool Evaluating { get; set; }

        public Individual(bool[] chromosome)
        {
            Chromosome = chromosome;
        }

        public Individual()
        {
            Chromosome = RandomChromosome();
        }

        public Individual[] Crossover(Individual other)
        {
            bool[] mixA = new bool[Chromosome.Length];
            bool[] mixB = new bool[Chromosome.Length];

            for (int i = 0; i < Chromosome.Length; i++)
            {
                if (i < Chromosome.Length / 2)
                {
                    mixA[i] = Chromosome[i];
                    mixB[i] = other.Chromosome[i];
                }
                else
                {
                    mixA[i] = other.Chromosome[i];
                    mixB[i] = Chromosome[i];
                }
            }

            return new Individual[2] { new Individual(mixA), new Individual(mixB) };
        }

        public void Mutate()
        {
            Chromosome[Random.Range(0, Chromosome.Length)] = !Chromosome[Random.Range(0, Chromosome.Length)];
        }

        private bool[] RandomChromosome()
        {
            var b = new bool[240];

            for (int i = 0; i < b.Length; i++)
            {
                b[i] = Random.Range(0, 2) == 0;
            }

            return b;
        }

        public int CompareTo(Individual other)
        {
            return Fitness.CompareTo(other.Fitness);
        }
    }

    public GameObject agentPrefab;
    public Transform spawnPoint;

    private Individual[] population;
    private GenetixPhase phase = GenetixPhase.Starting;

    private int populationSize = 200;
    private int populationTop = 20;

	// Use this for initialization
	void Start () {
        population = new Individual[populationSize];
        InitializePopulation();
        Evaluate();
    }

    void InitializePopulation()
    {
        phase = GenetixPhase.InitializePopulation;
        for (int i = 0; i < population.Length; i++)
        {
            population[i] = new Individual();
        }
    }

    void Evaluate()
    {
        phase = GenetixPhase.EvaluateFitness;

        for (int i = 0; i < population.Length; i++)
        {
            var agent = Instantiate(agentPrefab, spawnPoint.position, agentPrefab.transform.rotation).GetComponent<Agent>();
            agent.Individual = population[i];
            agent.Individual.Evaluating = true;
        }
    }

    // Update is called once per frame
    void Update () {
        if (phase == GenetixPhase.EvaluateFitness)
        {
            bool phaseOver = true;

            for (int i = 0; i < population.Length; i++)
            {
                if (population[i].Evaluating) phaseOver = false;
            }

            if (phaseOver) phase = GenetixPhase.SCM;
        }

        if (phase == GenetixPhase.SCM)
        {
            Selection();
            Crossover();
            Mutation();
            Evaluate();
        }
	}

    void Selection()
    {
        SortPopulation();

        //Remove lowest 90%
        for (int i = populationTop; i < population.Length; i++)
        {
            population[i] = null;
        }
    }

    void Crossover()
    {
        int currentFather = 0;
        int currentMother = 1;

        //Crossover individuals until population is filled
        for (int i = populationTop; i < population.Length; i += 2)
        {
            Individual[] offspring = population[currentFather].Crossover(population[currentMother]);

            population[i] = offspring[0];
            population[i + 1] = offspring[1];

            if (++currentMother >= populationTop)
            {
                currentFather += 1;
                currentMother = currentFather + 1;
            }
        }
    }

    void Mutation()
    {
        for (int i = 0; i < population.Length; i++)
        {
            if (Random.Range(0, 20) == 0) population[i].Mutate();
        }
    }

    void SortPopulation()
    {
        Array.Sort(population);
    }

    void PrintPopulation()
    {
        for (int i = 0; i < population.Length; i++)
        {
            Debug.Log(population[i].Fitness);
        }
    }
}
