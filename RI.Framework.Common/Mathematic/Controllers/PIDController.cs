using System.Diagnostics.CodeAnalysis;

using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Mathematic.Controllers
{
    /// <summary>
    ///     Implements a PID controller.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         PID controllers compute an output (<see cref="Output" />) which acts as a control value to control or drive a process.
    ///         The output is computed based on a desired value or outcome (called set point, <see cref="SetPoint" />) and the feedback or measurement from the process (called process variable, <see cref="ProcessVariable" />).
    ///         The calculation uses the error (set point minus process variable) and several coefficients (proportional <see cref="KP" />; integral <see cref="KI" />; differential <see cref="KD" />).
    ///         To deactivate a coefficient, its value can be set to 0.0f.
    ///     </para>
    ///     <para>
    ///         See <see href="https://en.wikipedia.org/wiki/PID_controller"> https://en.wikipedia.org/wiki/PID_controller </see> for details about PID controllers and their theory of operation.
    ///     </para>
    ///     <para>
    ///         When using a PID controller, be aware of steady-state-error, integral windup, noise sensitivity, and instability/oscillation.
    ///         To increase stability, the output can be clamped using <see cref="OutputMin" /> and <see cref="OutputMax" />.
    ///         Especially the differential component is sensitive to noise in the process variable, therefore the differential component is disabled by default (<see cref="KD" /> is 0.0).
    ///     </para>
    ///     <para>
    ///         The following values are used after a new instance of <see cref="PIDController" /> is created or the controller has been reset using <see cref="Reset" />:
    ///         <see cref="KP" />, <see cref="KI" /> are set to 1.0.
    ///         <see cref="KD" /> is set to 0.0.
    ///         <see cref="SetPoint" />, <see cref="ProcessVariable" />, <see cref="Output" />, <see cref="OutputUnclamped" /> are set to 0.0.
    ///         <see cref="OutputMin" /> is set to <see cref="float.MinValue" /> and <see cref="OutputMax" /> is set to <see cref="float.MaxValue" />, effectively disable clamping of the output.
    ///         <see cref="Error" />, <see cref="Integral" /> are set to 0.0.
    ///         <see cref="Loops" /> is set to 0.
    ///     </para>
    ///     <para>
    ///         The following values are updated with each computation: <see cref="SetPoint" /> (if changed), <see cref="ProcessVariable" />, <see cref="Output" />, <see cref="OutputUnclamped" />, <see cref="Error" />, <see cref="Integral" />, <see cref="Differential"/>, <see cref="Loops" />.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    /// <example>
    ///     <code language="cs">
    /// <![CDATA[
    /// // create the controller
    /// PIDController controller = new PIDController();
    /// 
    /// // set parameters
    /// controller.KP = 0.5f;
    /// controller.KI = 0.22f;
    /// controller.KD = 0.0f;
    /// 
    /// // ...
    /// 
    /// // update the controller once per frame
    /// // Time.deltaTime is used as timestep to ensure that the controller behaves framerate-independent
    /// float newSetting = controller.ComputeNewSetPoint(aim, feedback, Time.deltaTime);
    /// ]]>
    /// </code>
    /// </example>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public sealed class PIDController : IController
    {
        private float _outputMax;
        private float _outputMin;
        private float _kd;
        private float _ki;
        private float _kp;




        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="PIDController" />.
        /// </summary>
        public PIDController ()
        {
            this.Reset();
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the current differential value used for the differential component.
        /// </summary>
        /// <value>
        ///     The current differential value used for the differential component.
        /// </value>
        public float Differential { get; private set; }

        /// <summary>
        ///     Gets the current error (set point minus process variable).
        /// </summary>
        /// <value>
        ///     The current error (set point minus process variable).
        /// </value>
        public float Error { get; private set; }

        /// <summary>
        ///     Gets the current integral value used for the integral component.
        /// </summary>
        /// <value>
        ///     The current integral value used for the integral component.
        /// </value>
        public float Integral { get; private set; }

        /// <summary>
        ///     Gets or sets the differential coefficient.
        /// </summary>
        /// <value>
        ///     The differential coefficient.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="Output"/> and <see cref="OutputUnclamped"/> are not updated when changing the value of <see cref="KD"/>.
        ///     </note>
        /// </remarks>
        /// <exception cref="NotFiniteArgumentException"><paramref name="value"/> is NaN or infinity.</exception>
        public float KD
        {
            get => this._kd;
            set
            {
                if (value.IsNanOrInfinity())
                {
                    throw new NotFiniteArgumentException(nameof(value));
                }
                this._kd = value;
            }
        }

        /// <summary>
        ///     Gets or sets the integral coefficient.
        /// </summary>
        /// <value>
        ///     The integral coefficient.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="Output"/> and <see cref="OutputUnclamped"/> are not updated when changing the value of <see cref="KI"/>.
        ///     </note>
        /// </remarks>
        /// <exception cref="NotFiniteArgumentException"><paramref name="value"/> is NaN or infinity.</exception>
        public float KI
        {
            get => this._ki;
            set
            {
                if (value.IsNanOrInfinity())
                {
                    throw new NotFiniteArgumentException(nameof(value));
                }
                this._ki = value;
            }
        }

        /// <summary>
        ///     Gets or sets the proportional coefficient.
        /// </summary>
        /// <value>
        ///     The proportional coefficient.
        /// </value>
        /// <remarks>
        ///     <note type="implement">
        ///         <see cref="Output"/> and <see cref="OutputUnclamped"/> are not updated when changing the value of <see cref="KP"/>.
        ///     </note>
        /// </remarks>
        /// <exception cref="NotFiniteArgumentException"><paramref name="value"/> is NaN or infinity.</exception>
        public float KP
        {
            get => this._kp;
            set
            {
                if (value.IsNanOrInfinity())
                {
                    throw new NotFiniteArgumentException(nameof(value));
                }
                this._kp = value;
            }
        }

        #endregion




        #region Interface: IController

        /// <inheritdoc />
        public int Loops { get; private set; }

        /// <inheritdoc />
        public float Output { get; private set; }

        /// <inheritdoc />
        public float OutputMax
        {
            get => this._outputMax;
            set
            {
                if (value.IsNanOrInfinity())
                {
                    throw new NotFiniteArgumentException(nameof(value));
                }
                this._outputMax = value;
            }
        }

        /// <inheritdoc />
        public float OutputMin
        {
            get => this._outputMin;
            set
            {
                if (value.IsNanOrInfinity())
                {
                    throw new NotFiniteArgumentException(nameof(value));
                }
                this._outputMin = value;
            }
        }

        /// <inheritdoc />
        public float OutputUnclamped { get; private set; }

        /// <inheritdoc />
        public float ProcessVariable { get; private set; }

        /// <inheritdoc />
        public float SetPoint { get; private set; }

        /// <inheritdoc />
        public float Compute (float processVariable) => this.ComputeWithNewSetPoint(this.SetPoint, processVariable, 1.0f);

        /// <inheritdoc />
        public float Compute (float processVariable, float timestep) => this.ComputeWithNewSetPoint(this.SetPoint, processVariable, timestep);

        /// <inheritdoc />
        public float ComputeWithNewSetPoint (float setPoint, float processVariable) => this.ComputeWithNewSetPoint(setPoint, processVariable, 1.0f);

        /// <inheritdoc />
        public float ComputeWithNewSetPoint (float setPoint, float processVariable, float timestep)
        {
            if (setPoint.IsNanOrInfinity())
            {
                throw new NotFiniteArgumentException(nameof(setPoint));
            }

            if (processVariable.IsNanOrInfinity())
            {
                throw new NotFiniteArgumentException(nameof(processVariable));
            }

            if (timestep.IsNanOrInfinity())
            {
                throw new NotFiniteArgumentException(nameof(timestep));
            }

            this.Loops++;

            float error = setPoint - processVariable;
            float integral = this.Integral + (error * timestep);
            float differential = (error - this.Error) / timestep;

            float p = this.KP * error;
            float i = this.KI * integral;
            float d = this.KD * differential;

            float outputUnclamped = p + i + d;
            float output = outputUnclamped;
            if (output < this.OutputMin)
            {
                output = this.OutputMin;
            }
            if (output > this.OutputMax)
            {
                output = this.OutputMax;
            }

            this.SetPoint = setPoint;
            this.ProcessVariable = processVariable;
            this.Output = output;
            this.OutputUnclamped = outputUnclamped;
            this.Error = error;
            this.Integral = integral;
            this.Differential = differential;

            return output;
        }

        /// <inheritdoc />
        public void Reset ()
        {
            this.Loops = 0;

            this.KP = 1.0f;
            this.KI = 1.0f;
            this.KD = 0.0f;

            this.SetPoint = 0.0f;
            this.ProcessVariable = 0.0f;
            this.Output = 0.0f;
            this.OutputUnclamped = 0.0f;

            this.OutputMin = float.MinValue;
            this.OutputMax = float.MaxValue;

            this.Error = 0.0f;
            this.Integral = 0.0f;
            this.Differential = 0.0f;
        }

        #endregion
    }
}
