#!/usr/bin/env bash
mpm install
mpm build --input "src/attribute-util" --output "out/AttributeUtil.cs"
mpm build --input "src/coordinate-service" --output "out/CoordinateService.cs"
mpm build --input "src/maid" --output "out/Maid.cs"
mpm build --input "src/noise" --output "out/Noise.cs"
mpm build --input "src/option" --output "out/Option.cs"
mpm build --input "src/player-util" --output "out/PlayerUtil.cs"
mpm build --input "src/signal" --output "out/Signal.cs"
mpm build --input "src/spawn-service" --output "out/SpawnService.cs"
mpm build --input "src/step-service" --output "out/StepService.cs"
mpm build --input "src/tag-service" --output "out/TagService.cs"
mpm build --input "src/task-util" --output "out/TaskUtil.cs"
mpm build --input "src/test-service" --output "out/TestService.cs"