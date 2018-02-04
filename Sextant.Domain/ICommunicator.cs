// Copyright (c) Stickymaddness All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Sextant.Domain
{
    public interface ICommunicator
    {
        void Initialize();
        void Communicate(string message);
        void StopComminicating();
    }
}
