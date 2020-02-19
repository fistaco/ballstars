using System;

namespace TeamBuilder.Entity.Individual
{
    internal abstract class Individual
    {
        /// <summary>
        /// The current fitness evaluation of this individual.
        /// </summary>
        public float Fitness;

        /// <summary>
        /// Evaluates this individual, sets its fitness property and returns the resulting fitness value.
        /// </summary>
        /// <returns>The fitness value resulting from the evaluation.</returns>
        public abstract float Evaluate();

        /// <summary>
        /// Combines this individual with another given individual.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public abstract Individual Crossover(Individual other);

        /// <summary>
        /// Changes one or multiple of this individual's properties.
        /// </summary>
        public abstract void Mutate();
    }
}
