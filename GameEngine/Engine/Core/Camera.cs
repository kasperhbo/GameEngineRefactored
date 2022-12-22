using OpenTK.Mathematics;

namespace GameEngine.Engine.Core;

public class Camera
{
 // Those vectors are directions pointing outwards from the camera to define how it rotated.
        private readonly Vector3 _front = -Vector3.UnitZ;
        private readonly Vector3 _up = Vector3.UnitY;
        private bool _firstMove = true;
        // The field of view of the camera (radians)
        private float _fov = MathHelper.PiOver2;
        private Vector2 _lastPos;
        // Rotation around the X axis (radians)
        private float _pitch;
        private Vector3 _right = Vector3.UnitX;
        // Rotation around the Y axis (radians)
        private float _yaw = -MathHelper.PiOver2; // Without this, you would be started rotated 90 degrees right.
        public Vector2 position;

        private Matrix4 inverseProjection, inverseView, projectionMatrix, viewMatrix;

        private System.Numerics.Vector2 projectSize = new System.Numerics.Vector2(32 * 40, 32 * 21);

        public Camera(Vector3 position) 
        {
            this.position = new Vector2(position.X, position.Y);
            projectionMatrix = new Matrix4();
            viewMatrix = new Matrix4();
            inverseProjection = new Matrix4();
            inverseView = new Matrix4();
            AdjustProjection();
        }

        public void AdjustProjection()
        {
            // projectionMatrix = Matrix4.Identity;
            projectionMatrix = Matrix4.CreateOrthographicOffCenter(0, projectSize.X, 0, projectSize.Y, 0, 100f);
            Matrix4.Invert(projectionMatrix, out inverseProjection);
        }

        public Matrix4 GetViewMatrix()
        {
            viewMatrix = Matrix4.LookAt(new Vector3(position.X, position.Y, 0), new Vector3(position.X, position.Y, 0) + _front, _up);
            Matrix4.Invert(viewMatrix, out inverseView);
            return viewMatrix;
        }

        public Matrix4 GetProjectionMatrix()
        {
            return projectionMatrix;
        }

        public Matrix4 GetInverseProjection()
        {
            return inverseProjection;
        }

        public Matrix4 GetInverseView()
        {
            return inverseView;
        }

        public System.Numerics.Vector2 GetProjectSize()
        {
            return projectSize;
        }   
}