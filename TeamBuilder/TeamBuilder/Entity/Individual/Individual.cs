using System;
using System.Collections.Generic;

namespace TeamBuilder.Entity.Individual
{
    internal abstract class Individual
    {
        /// <summary>
        /// The current fitness evaluation of this individual.
        /// </summary>
        public float Fitness;

        /// <summary>
        /// Evaluates this individual using a given fitness function, sets the individual's fitness property and
        /// returns the resulting fitness value.
        /// </summary>
        /// <param name="fitnessFunc"></param>
        /// <returns>The fitness value resulting from the evaluation.</returns>
        public float Evaluate(Func<Individual, float> fitnessFunc)
        {
            this.Fitness = fitnessFunc(this);

            return this.Fitness;
        }

        /// <summary>
        /// Combines this individual with another given individual.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public abstract Individual Combine(Individual other);

        /// <summary>
        /// Changes one or multiple of this individual's properties.
        /// </summary>
        public abstract void Mutate();
    }
}
