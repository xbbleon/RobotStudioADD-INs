using System;
using System.Collections.Generic;
using System.Text;

using ABB.Robotics.Math;
using ABB.Robotics.RobotStudio;
using ABB.Robotics.RobotStudio.Environment;
using ABB.Robotics.RobotStudio.Stations;

//	To activate this Add-in you have to copy EnergyRead.rsaddin to the Add-In directory,
//  typically C:\Program Files (x86)\Common Files\ABB Industrial IT\Robotics IT\RobotStudio\AddIns

namespace EnergyRead
{
    public class Class1
    {
        public static List<string> energy_list = new List<string>();
        public static List<string> time_list = new List<string>();
        public static List<string> speed_list = new List<string>();
        // This is the entry point which will be called when the Add-in is loaded
        public static void AddinMain()
        {
            Logger.AddMessage(new LogMessage("The Add-in is already excute!"));
            CreateButton();
        }
        //首先创建一个ribbonTab,功能栏的一个选项
        //接着创建一个ribbonGroup 是button的group
        //创建button 设定属性 放入ribbonGroup
        //对每个button设定相应的处理事件。
        private static void CreateButton()
        {
            Project.UndoContext.BeginUndoStep("Add Buttons");
            try
            {   //在功能栏添加了一个叫Mytab的选项
                RibbonTab ribbonTab = new RibbonTab("MyTab", "MyAdd-in");
                UIEnvironment.RibbonTabs.Add(ribbonTab);
                UIEnvironment.ActiveRibbonTab = ribbonTab;

                RibbonGroup ribbonGroup = new RibbonGroup("Mybutton", "Status");

                // Create first small button （id，text）
                CommandBarButton buttonFirst = new CommandBarButton("MyFirstButton", "OpenStatusWindow");
                buttonFirst.HelpText = "Help text for small button";
                // buttonFirst.Image = global::RobotStudioEmptyAddin1.Properties.Resources.iconCFimport;
                buttonFirst.DefaultEnabled = true;
                ribbonGroup.Controls.Add(buttonFirst);

                //Include Seperator between buttons
                CommandBarSeparator seperator = new CommandBarSeparator();
                ribbonGroup.Controls.Add(seperator);

                // Create second button. The largeness of the button is determined by RibbonControlLayout
                CommandBarButton buttonSecond = new CommandBarButton("MySecondButton", "TestButton");
                buttonSecond.HelpText = "Help text for large button";
                buttonSecond.DefaultEnabled = true;
                ribbonGroup.Controls.Add(buttonSecond);

                // Set the size of the buttons.
                RibbonControlLayout[] ribbonControlLayout = { RibbonControlLayout.Small, RibbonControlLayout.Large };
                ribbonGroup.SetControlLayout(buttonFirst, ribbonControlLayout[0]);
                ribbonGroup.SetControlLayout(buttonSecond, ribbonControlLayout[1]);



                //Add ribbon group to ribbon tab
                ribbonTab.Groups.Add(ribbonGroup);

                // Add an event handler.
                buttonFirst.UpdateCommandUI += new UpdateCommandUIEventHandler(button_UpdateCommandUI);
                // Add an event handler for pressing the button.
                buttonFirst.ExecuteCommand += new ExecuteCommandEventHandler(button_ExecuteCommand);

                buttonSecond.UpdateCommandUI += new UpdateCommandUIEventHandler(button2_UpdateCommandUI);
                buttonSecond.ExecuteCommand += new ExecuteCommandEventHandler(button2_ExecuteCommand);
            }
            catch (Exception ex)
            {
                Project.UndoContext.CancelUndoStep(CancelUndoStepType.Rollback);
                Logger.AddMessage(new LogMessage(ex.Message.ToString()));
            }
            finally
            {
                Project.UndoContext.EndUndoStep();
            }
        }
        //button1事件处理
        static void button_ExecuteCommand(object sender, ExecuteCommandEventArgs e)
        {
            Logger.AddMessage(new LogMessage("按钮方法执行了"));
            // CreateTargets();
            ToolWindow tw = new ToolWindow();
            tw.PreferredSize = new System.Drawing.Size(450, 150);
            UserControl1 u1 = new UserControl1();
            tw.Control = u1;
            tw.Caption = "测试";
            UIEnvironment.Windows.AddDocked(tw, System.Windows.Forms.DockStyle.Right);

            //copyTarget();
        }

        static void button_UpdateCommandUI(object sender, UpdateCommandUIEventArgs e)
        {
            // This enables the button, instead of "button1.Enabled = true".
            e.Enabled = true;
        }

        //button2
        static void button2_ExecuteCommand(object sender, ExecuteCommandEventArgs e)
        {
            Logger.AddMessage(new LogMessage("This method is excuted"));
        }

        static void button2_UpdateCommandUI(object sender, UpdateCommandUIEventArgs e)
        {
            // This enables the button, instead of "button1.Enabled = true".
            e.Enabled = true;
        }

        //创建虚拟信号
        public static void VirtualSignals()
        {
            #region VirtualSignals Example
            Project.UndoContext.BeginUndoStep("VirtualSignals");
            try
            {
                Station station = Project.ActiveProject as Station;
                #region ISStep1
                if (Simulator.DataRecorder.Sinks.Contains("SignalSink"))
                {
                    Simulator.DataRecorder.Sinks.Remove(Simulator.DataRecorder.Sinks["SignalSink"]);
                }
                #endregion

                #region ISStep2

                Simulator.DataRecorder.Sinks.Add(new DataRecorderSink("SignalSink"));
                #endregion
                DataRecorderSink signalSink = (DataRecorderSink)Simulator.DataRecorder.Sinks["SignalSink"];
                BuiltInControllerSourceSignals signals = station.BuiltInDataRecorderSignals.ControllerSignals;
                signalSink.Enabled = true;
                signalSink.DataRecorder.Start();

                RsTask task = station.ActiveTask;
                RsIrc5Controller rsIrc5Controller = (RsIrc5Controller)task.Parent;
                ABB.Robotics.Controllers.Controller controller =
                       new ABB.Robotics.Controllers.Controller(new Guid(rsIrc5Controller.SystemId.ToString()));
                Logger.AddMessage("isVitural:" + controller.IsVirtual);

                BuiltInDataRecorderMotionSignal energy = BuiltInDataRecorderMotionSignal.TotalMotorPowerConsumption;
                BuiltInDataRecorderMotionSignal speed = BuiltInDataRecorderMotionSignal.TCPSpeedInCurrentWorkObject;
                // DataRecorderSignal dr = signals.GetMotionSignal(new Guid(rsIrc5Controller.SystemId.ToString()), controller.MotionSystem.ActiveMechanicalUnit.Name, energy);
                RsIrc5ControllerCollection controllerlist = station.Irc5Controllers;
                foreach (RsIrc5Controller irc5Controller in controllerlist)
                {

                    DataRecorderSignal energy_consumption = signals.GetMotionSignal(new Guid(rsIrc5Controller.SystemId.ToString()), controller.MotionSystem.ActiveMechanicalUnit.Name, energy);
                    DataRecorderSignal TcpSpeed = signals.GetMotionSignal(new Guid(rsIrc5Controller.SystemId.ToString()), controller.MotionSystem.ActiveMechanicalUnit.Name, speed);
                    if (TcpSpeed != null && !signalSink.Signals.Contains(TcpSpeed) && energy_consumption != null && !signalSink.Signals.Contains(energy_consumption))
                    {
                        signalSink.Signals.Add(energy_consumption);
                        signalSink.Signals.Add(TcpSpeed);
                        Logger.AddMessage("TCP_ID:" + TcpSpeed.Id);
                        Logger.AddMessage("TCP_DisplayPath:" + TcpSpeed.DisplayPath);
                        Logger.AddMessage("ENERGY_ID:" + energy_consumption.Id);
                        Logger.AddMessage("ENERGY_DisplayPath:" + energy_consumption.DisplayPath);
                    }
                    else
                    {
                        Logger.AddMessage("TcpSpeed为空");
                    }

                }

            }
            catch (Exception ex)
            {
                Logger.AddMessage(new LogMessage(ex.Message.ToString()));
                Project.UndoContext.CancelUndoStep(CancelUndoStepType.Rollback);

            }
            finally
            {
                Project.UndoContext.EndUndoStep();
            }
            #endregion
        }
        public class DataRecorderSink : DataRecorderSinkBase
        {

            #region Constructor
            /// <summary>
            /// Default Constructor
            /// </summary>
            /// <param name="sinkId"></param>
            public DataRecorderSink(string sinkId)
                : base(sinkId)
            { }
            #endregion

            #region OnData
            protected override void OnData(double time, DataRecorderSignal signal, object value)
            {
                try
                {
                    string[] signalId = signal.Id.ToString().Split('\\');
                    /*  for (int i = 0; i < signalId.Length; i++)
                      {
                          Logger.AddMessage(signalId[i]);
                      }*/
                    //Logger.AddMessage("时间为" + time.ToString());
                    time_list.Add(time.ToString());


                    if (signalId[signalId.Length - 1].ToString().Equals("3"))
                    {
                        //Logger.AddMessage(new LogMessage("能耗值为：" + value.ToString()));
                        energy_list.Add(value.ToString());
                    }
                    else if (signalId[signalId.Length - 1].ToString().Equals("9878"))
                    {
                        //Logger.AddMessage("速度为" + value.ToString());
                        if (value == null || value.ToString() == "0")
                        {
                            speed_list.Add("0");
                        }
                        else
                        {
                            speed_list.Add(value.ToString());
                        }

                    }
                    else
                    {
                        Logger.AddMessage("未找到");
                    }
                }
                catch (Exception ex)
                {

                    Logger.AddMessage(new LogMessage(ex.Message.ToString()));
                }

            }
            #endregion
        }
    }
}