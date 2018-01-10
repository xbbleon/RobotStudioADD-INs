using System;
using System.Windows.Forms;
using ABB.Robotics.RobotStudio.Stations;
using System.Threading.Tasks;

using ABB.Robotics.Math;
using ABB.Robotics.RobotStudio;
using System.Timers;
using ABB.Robotics.RobotStudio.Stations.Forms;
using System.Collections;
using System.Collections.Generic;
using ABB.Robotics.Controllers.RapidDomain;


namespace EnergyRead
{


    public partial class UserControl1 : UserControl
    {

        private System.Windows.Forms.Timer timer1;
        public UserControl1()
        {
            InitializeComponent();
            //     this.timer = new System.Timers.Timer(500);
            //    timer.Elapsed += new System.Timers.ElapsedEventHandler(ReadData);
            //      timer.AutoReset = true;

            // ReadData();

            timer1 = new System.Windows.Forms.Timer();
            timer1.Enabled = true;
            timer1.Interval = 500;
            timer1.Tick += new EventHandler(ReadData);
        }


        private void ReadData(object sender, System.EventArgs e)
        {
            try
            {
                Station station = Project.ActiveProject as Station;
                RsToolData tool = station.ActiveTask.ActiveTool;
                Vector4 trans = tool.Frame.GlobalMatrix.t;
                // x,y,z
                trans.x *= 1000.0;
                trans.y *= 1000.0;
                trans.z *= 1000.0;
                //rx ry rz
                double rx = Globals.RadToDeg(tool.Frame.GlobalMatrix.EulerXYZ.x);
                double ry = Globals.RadToDeg(tool.Frame.GlobalMatrix.EulerXYZ.y);
                double rz = Globals.RadToDeg(tool.Frame.GlobalMatrix.EulerXYZ.z);

                this.textBox_x.Text = trans.x.ToString();
                this.textBox_y.Text = trans.y.ToString();
                this.textBox_z.Text = trans.z.ToString();


                this.textBox_RX.Text = rx.ToString();
                this.textBox_RY.Text = ry.ToString();
                this.textBox_RZ.Text = rz.ToString();



                Mechanism mech = station.ActiveTask.Mechanism;
                double[] jointValues = mech.GetJointValues();



                this.textBox1.Text = Globals.RadToDeg(jointValues[0]).ToString();
                this.textBox2.Text = Globals.RadToDeg(jointValues[1]).ToString();
                this.textBox3.Text = Globals.RadToDeg(jointValues[2]).ToString();
                this.textBox4.Text = Globals.RadToDeg(jointValues[3]).ToString();
                this.textBox5.Text = Globals.RadToDeg(jointValues[4]).ToString();
                this.textBox6.Text = Globals.RadToDeg(jointValues[5]).ToString();

                this.textBox7.Text = Class1.energy_list[Class1.energy_list.Count-1];

            }
            catch (Exception)
            {
                timer1.Enabled = true;
                timer1.Interval = 500;
            }



        }


        private void button2_Click(object sender, EventArgs e)
        {
            Class1.VirtualSignals();
        }
    }
}
