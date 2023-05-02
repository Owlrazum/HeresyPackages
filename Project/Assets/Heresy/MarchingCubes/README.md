# MarchingCubes

## Desciption

The main script is `MarchingCubes.cs`, containing the `Burst`-compatible job for generation of mesh using marching-cubes algorihtm, with supplied scalar field. An example of how to initialize the scalar field can be found at `TestMarchingCubes.cs`

## Acknowledgements

Was made thanks to terrain [implementation](https://github.com/Eldemarkki/Marching-Cubes-Terrain) and a [paper](http://paulbourke.net/geometry/polygonise/)

## Possible improvements

- Do not create vertices at the same position to allow smooth shading. Possible way is to remember which corners were used for interpolation, and instead of generating new vertex, retrieve relevant index.