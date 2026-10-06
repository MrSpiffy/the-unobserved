"""Seamless, original albedo/normal/roughness assets; no downloaded images."""
from pathlib import Path
import numpy as np
from PIL import Image
ROOT=Path(__file__).resolve().parents[1]/'Assets/Resources/Surfaces'
ROOT.mkdir(parents=True,exist_ok=True)
N=1024
rng=np.random.default_rng(404)
y,x=np.mgrid[:N,:N]/N
def noise(scale):
    a=rng.normal(size=(N,N))
    f=np.fft.fftfreq(N)
    filt=np.exp(-(f[:,None]**2+f[None,:]**2)*(N/scale)**2)
    v=np.fft.ifft2(np.fft.fft2(a)*filt).real
    return v/(np.std(v)+1e-8)
fine=noise(160); medium=noise(24); broad=noise(5)
def save(name,base,height,rough,metal=0,ao=None):
    if base.ndim==2: base=np.repeat(base[:,:,None],3,axis=2)
    base=np.clip(base,0,1)
    dx=(np.roll(height,-1,1)-np.roll(height,1,1))*5
    dy=(np.roll(height,-1,0)-np.roll(height,1,0))*5
    normal=np.stack((-dx,dy,np.ones_like(dx)),2)
    normal/=np.linalg.norm(normal,axis=2)[:,:,None]
    packed=np.stack((np.ones((N,N)) if ao is None else ao,np.broadcast_to(rough,(N,N)),np.broadcast_to(metal,(N,N))),2)
    for suffix,array in [('albedo',base),('normal',normal*.5+.5),('surface',packed)]:
        Image.fromarray((np.clip(array,0,1)*255).astype('uint8')).save(ROOT/f'{name}_{suffix}.png')

pores=(fine< -1.9).astype(float)
concrete=.63+medium*.028+broad*.022+fine*.008-pores*.11
save('concrete',concrete,medium*.08+fine*.04-pores*.12,np.clip(.84+medium*.03,.6,1),ao=1-pores*.15)
plaster=.72+medium*.013+broad*.012+fine*.003
save('plaster',plaster,medium*.015+fine*.015,.85)
grid=np.maximum(np.abs((x*4)%1-.5),np.abs((y*4)%1-.5))
grout=np.clip((grid-.477)/.012,0,1)
tile=.72+medium*.015+broad*.012-grout*.31
save('tile',tile,.15*(1-grout)+fine*.006,.32+grout*.55,ao=1-grout*.16)
weave=np.sin(2*np.pi*x*256)*np.sin(2*np.pi*y*256)
carpet=.51+weave*.033+fine*.018+medium*.016+broad*.014
save('carpet',carpet,weave*.08+fine*.03,.97)
grain=np.sin(2*np.pi*(x*24+np.sin(2*np.pi*y*2)*.1+medium*.028))
wood=.58+grain*.05+medium*.012+broad*.022
save('wood',np.stack((wood*1.12,wood*.97,wood*.79),2),grain*.035+fine*.005,.5)
grid=np.minimum(np.abs(((x+y)*8)%1-.5),np.abs(((x-y)*8)%1-.5))
ridge=np.clip((.06-grid)/.03,0,1)
metal=.49+ridge*.12+fine*.008+medium*.012
save('metal',metal,ridge*.13+fine*.004,.42,metal=.75,ao=.94+ridge*.06)
waves=np.sin(2*np.pi*(x*6+np.sin(2*np.pi*y*3)*.17))+np.sin(2*np.pi*(y*9+x*2))*.3
save('water',.52+waves*.015,waves*.07,.1,metal=.25)
print('Generated',len(list(ROOT.glob('*.png'))),'1024px seamless maps.')
