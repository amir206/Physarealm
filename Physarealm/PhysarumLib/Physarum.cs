﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper;
using Grasshopper.Kernel.Types;
using Rhino;
using Rhino.Geometry;
using Physarealm.Environment;

namespace Physarealm
{
    public class Physarum:GH_Goo<object>, IDisposable
    {
        public List<Amoeba> population = new List<Amoeba>();
        public List<Amoeba> _toborn_population = new List<Amoeba>();
        public List<Amoeba> _todie_population = new List<Amoeba>();
        public int _popsize { get; set; }
        public float _sense_offset { get; set; }
        public float _sense_angle { get; set; }
        public float _rotate_angle { get; set; }
        //private bool osc;
        public float _pcd { get; set; }
        //private float oscresetprob;
        public float _depT { get; set; }
        public float _speed { get; set; }
        public int _detectDir { get; set; }
        public int _death_distance { get; set; }
        public int _division_frequency_test { get; set; }
        public int _death_frequency_test { get; set; }
        //private float _division_prob;
        //private float _death_random_prob;
        private int gw;
        private int gmin;
        private int gmax;
        private int sw;
        private int smax;
        private int smin;
        //private int divisionborder;
        static private Libutility util = new Libutility();
        public int _current_population;
        private int _step;
        public double guide_factor { get; set; }

        public Physarum()
            : base()
        {
            _current_population = 0;
            _step = 0;
            _division_frequency_test = 4;
            _death_frequency_test = 4;
            guide_factor = 0.2;
            _popsize = 200;
        }
        public Physarum(Physarum p) :base()
        {
            population = p.population;
            _popsize  = p._popsize;
            _sense_offset = p._sense_offset;
            _sense_angle = p._sense_angle;
            _rotate_angle = p._rotate_angle;
            _pcd = p._pcd;
            _depT = p._depT;
            _speed = p._speed;
            _detectDir  = p._detectDir;
            _death_distance  = p._death_distance;
            _division_frequency_test = p._division_frequency_test;
            _death_frequency_test = p._death_frequency_test;
            gw = p.gw;
            gmin = p.gmin;
            gmax = p.gmax;
            sw = p.sw;
            smax = p.smax;
            smin = p.smin;
            guide_factor = p.guide_factor;

        
        }
        public void initParameters(float sensor_angle = (float) 22.5, float rotate_angle = 45, float sensor_offset = 7, int detectDir = 3, int deathDistance = 100, float max_speed = 3, float pcd = (float) 0.1, float dept = 3)
        {
            _sense_angle = sensor_angle;
            _rotate_angle = rotate_angle;
            _sense_offset = sensor_offset;
            _detectDir = detectDir;
            _death_distance = deathDistance;
            _speed = max_speed;
            _pcd = pcd;
            _depT = dept;
            _popsize = 50;

        }
        public void initPopulation(AbstractEnvironmentType env)
        {
            //_popsize = popsize;
            for (int i = 0; i < _popsize; i++)
            {
                Amoeba newAmo = new Amoeba(i, _sense_angle, _rotate_angle, _sense_offset, _detectDir, _death_distance, _speed, _pcd, _depT);
                Point3d birthPlace = env.getRandomBirthPlace(util);
                newAmo.initializeAmoeba((int)birthPlace.X, (int)birthPlace.Y, (int)birthPlace.Z, env.u, env.v, env.w, 4, env, util);
                newAmo._guide_factor = guide_factor;
                population.Add(newAmo);
                _current_population++;
            }
        }
        public void updatePopulation(AbstractEnvironmentType env)
        {
            //shuffleOrder();
            System.Threading.Tasks.Parallel.ForEach(population, amo =>
            {
                amo.doMotorBehaviors(env, util);
            });


            /*foreach (Amoeba amo in population)
            {
              amo.doMotorBehaviors(_grid, util);
              //amo.doSensortBehaviors(_grid, util);
            }
            //shuffleOrder();
            foreach (Amoeba amo in population)
            {
              amo.doSensortBehaviors(_grid, util);
              //amo.doMotorBehaviors(_grid, util);
            }*/
            System.Threading.Tasks.Parallel.ForEach(population, amo =>
            {
                amo.doSensortBehaviors(env, util);
            });

        }
        public void doDivisionTest(AbstractEnvironmentType env)
        {
            //shuffleOrder();
            /*
            System.Threading.Tasks.Parallel.ForEach(population, amo =>
            {
            if (amo._divide)
            {
            birthNew(amo);
            //_current_population++;
            }
            amo._divide = false;
            });*/

            foreach (Amoeba amo in population)
            {
                if (amo._divide)
                {
                    birthNew(amo, env);
                    //_current_population++;
                }
                amo._divide = false;
            }
        }
        public void updateTrails(AbstractEnvironmentType env)
        {
            env.projectToTrail();
            env.diffuseTrails();
        }
        public void doDeathTest(AbstractEnvironmentType env)
        {
            //shuffleOrder();
            /*
            System.Threading.Tasks.Parallel.ForEach(population, amo =>
            {
            if (amo._die )
            {
            _grid.clearGridCell(amo.curx, amo.cury, amo.curz);
            _todie_population.Add(amo);
            //population.Remove(amo);
            _current_population--;
            }
            });
            */

            foreach (Amoeba amo in population)
            {
                if (amo._die)
                {
                    env.clearGridCell(amo.curx, amo.cury, amo.curz);
                    _todie_population.Add(amo);
                    //population.Remove(amo);
                    _current_population--;
                }

            }
        }
        public void birthNew(Amoeba agent, AbstractEnvironmentType env)
        {
            if (agent.curx <= 4 || agent.curx >= env.u - 4 || agent.cury <= 4 || agent.cury >= env.v - 4 || agent.curz <= 4 || agent.curz >= env.w - 4)
                return;
            //if (agent.curx == agent.cury || agent.curx == agent.curz)
            //  return;
            Point3d newPos = env.getNeighbourhoodFreePos(agent.curx, agent.cury, agent.curz, 1, util);
            if (newPos.X == -1 || newPos.Y == -1 || newPos.Y == -1)
                return;
            _current_population++;
            int thisindex = _current_population - 1;
            Amoeba newAmo = new Amoeba(thisindex, _sense_angle, _rotate_angle, _sense_offset, _detectDir, _death_distance, _speed, _pcd, _depT);
            newAmo.initializeAmoeba((int)newPos.X, (int)newPos.Y, (int)newPos.Z, env, util);
            //newAmo.initializeAmoeba(agent.curx, agent.cury, agent.curz, 2, _grid, util);
            newAmo._guide_factor = guide_factor;
            newAmo.selectRandomDirection(util, agent.orientation);
            //Amoeba newAmo = new Amoeba(_current_population - 1, _sense_angle, _rotate_angle, _sense_offset, _detectDir, _death_distance, _speed, _pcd, _depT);
            //Point3d birthPlace = _grid.getRandomBirthPlace(util);
            //newAmo.initializeAmoeba((int) birthPlace.X, (int) birthPlace.Y, (int) birthPlace.Z, 3, _grid, util);
            //newAmo._guide_factor = guide_factor;
            _toborn_population.Add(newAmo);

        }
        public void shuffleOrder()
        {
            Random rd = new Random((int)DateTime.Now.Ticks);
            population.OrderBy(x => rd.Next());
        }
        public void Update(AbstractEnvironmentType env)
        {
            updatePopulation(env);
            updateTrails(env);
            if (_step % _death_frequency_test == 0)
                doDeathTest(env);
            if (_step % _division_frequency_test == 0)
                doDivisionTest(env);
            foreach (Amoeba tb in _toborn_population)
                population.Add(tb);
            foreach (Amoeba td in _todie_population)
                population.Remove(td);
            _toborn_population = new List<Amoeba>();
            _todie_population = new List<Amoeba>();
            _step++;
        }
        public void setBirthDeathCondition(List<int> cond)
        {
            gw = cond[0];
            gmin = cond[1];
            gmax = cond[2];
            sw = cond[3];
            smin = cond[4];
            smax = cond[5];
        }
        public void Clear() 
        {
            population.Clear();
            _toborn_population.Clear();
            _todie_population.Clear();
        
        }
        public void Dispose()
        {
            Clear();
        }

        public override IGH_Goo Duplicate()
        {
            return new Physarum(this);
        }

        public override bool IsValid
        {
            get { return true; }
        }

        public override string ToString()
        {

            return TypeName + "\n current population: " + population.Count;
        }

        public override string TypeDescription
        {
            get { return "Physarealm.Physarum"; }
        }

        public override string TypeName
        {
            get { return "Physarealm.Physarum"; }
        }
    }//end of Phy
}
