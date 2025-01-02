// Camera.cs
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Keys = OpenTK.Windowing.GraphicsLibraryFramework.Keys;

namespace SimpleGridFly
{
    public class Camera
    {
        // Camera Attributes
        public Vector3 Position { get; private set; }

        public Vector3 Front { get; private set; }
        public Vector3 Up { get; private set; }
        public Vector3 Right { get; private set; }
        public Vector3 WorldUp { get; private set; }

        // Euler Angles
        public float Yaw { get; private set; }

        public float Pitch { get; private set; }

        // Camera options
        private float _speed = 10f;

        private float _sensitivity = 0.2f;
        private float _zoom = 60f;

        // Projection matrix
        private Matrix4 _projectionMatrix;

        public Camera(Vector3 position, float yaw, float pitch, float aspectRatio)
        {
            Position = position;
            WorldUp = Vector3.UnitY;
            Yaw = yaw;
            Pitch = pitch;
            Front = -Vector3.UnitZ; // Default front vector
            UpdateCameraVectors();
            _projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(_zoom), aspectRatio, 0.1f, 10000f);
        }

        /// <summary>
        /// Returns the view matrix calculated using Euler Angles and the LookAt Matrix
        /// </summary>
        public Matrix4 GetViewMatrix()
        {
            var view = Matrix4.LookAt(Position, Position + Front, Up);
            var mirrorZ = Matrix4.CreateScale(1, 1, -1); // Flip Z-axis
            return view * mirrorZ;

            // return Matrix4.LookAt(Position, Position + Front, Up);
        }

        /// <summary>
        /// Returns the projection matrix
        /// </summary>
        public Matrix4 GetProjectionMatrix()
        {
            return _projectionMatrix;
        }

        /// <summary>
        /// Processes input received from any keyboard-like input system.
        /// Accepts input parameter in the form of camera defined ENUM (to abstract it from windowing systems)
        /// </summary>
        public void ProcessKeyboard(KeyboardState keyboard, float deltaTime)
        {
            if (keyboard.IsKeyDown(Keys.LeftShift))
            {
                _speed = 10000f;
            }
            else
            {
                _speed = 1000f;
            }
            float velocity = _speed * deltaTime;
            if (keyboard.IsKeyDown(Keys.W))
                Position -= Front * velocity;
            if (keyboard.IsKeyDown(Keys.S))
                Position += Front * velocity;
            if (keyboard.IsKeyDown(Keys.A))
                Position -= Right * velocity;
            if (keyboard.IsKeyDown(Keys.D))
                Position += Right * velocity;
            if (keyboard.IsKeyDown(Keys.Q))
                Position -= WorldUp * velocity;
            if (keyboard.IsKeyDown(Keys.E))
                Position += WorldUp * velocity;
        }

        /// <summary>
        /// Processes input received from a mouse input system.
        /// Expects the offset value in both the x and y direction.
        /// </summary>
        public void ProcessMouseDrag(float deltaX, float deltaY)
        {
            Yaw -= deltaX * _sensitivity;
            Pitch -= deltaY * _sensitivity;

            // Make sure that when pitch is out of bounds, screen doesn't get flipped
            Pitch = MathHelper.Clamp(Pitch, -89f, 89f);

            // Update Front, Right and Up Vectors using the updated Euler angles
            UpdateCameraVectors();
        }

        /// <summary>
        /// Updates the camera's Front, Right, and Up vectors based on the current Yaw and Pitch
        /// </summary>
        private void UpdateCameraVectors()
        {
            // Calculate the new Front vector
            Vector3 front;
            front.X = MathF.Cos(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
            front.Y = MathF.Sin(MathHelper.DegreesToRadians(Pitch));
            front.Z = -MathF.Sin(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
            Front = Vector3.Normalize(front);

            // Also re-calculate the Right and Up vector
            Right = Vector3.Normalize(Vector3.Cross(Front, WorldUp));  // Normalize the vectors, because their length gets closer to 0 the more you look up or down which results in slower movement.
            Up = Vector3.Normalize(Vector3.Cross(Right, Front));
        }

        /// <summary>
        /// Updates the projection matrix when the aspect ratio changes
        /// </summary>
        public void UpdateAspectRatio(float aspectRatio)
        {
            _projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(_zoom), aspectRatio, 0.1f, 10000f);
        }
    }
}