"""Original offline sound design. Requires numpy; emits reproducible PCM source assets."""
from pathlib import Path
import wave
import numpy as np

ROOT=Path(__file__).resolve().parents[1]/'Assets/Resources/Audio'
ROOT.mkdir(parents=True,exist_ok=True)
SR=22050
rng=np.random.default_rng(9172)
def noise(n,lo=30,hi=5000,slope=.5):
    f=np.fft.rfftfreq(n,1/SR)
    spectrum=rng.normal(size=len(f))+1j*rng.normal(size=len(f))
    spectrum*=((f>=lo)&(f<=hi))/np.maximum(f,1)**slope
    x=np.fft.irfft(spectrum,n=n)
    return x/(np.std(x)+1e-9)
def save(name,x,peak=.8):
    x=np.asarray(x)
    if x.ndim==1: x=x[:,None]
    x=x*min(1,peak/(np.max(np.abs(x))+1e-9))
    with wave.open(str(ROOT/(name+'.wav')),'wb') as w:
        w.setnchannels(x.shape[1]); w.setsampwidth(2); w.setframerate(SR)
        w.writeframes((x*32767).astype('<i2').tobytes())
def wet(x,amount=.2):
    y=np.column_stack((x.copy(),x.copy()))
    for delay,gain,pan in [( .071,.32,0),(.113,.27,1),(.197,.19,0),(.263,.16,1),(.431,.11,0),(.587,.07,1)]:
        k=int(delay*SR)
        y[k:,pan]+=x[:-k]*gain*amount
    return y

for surface in ['carpet','stone','metal']:
    for v in range(4):
        t=np.arange(int(SR*.48))/SR
        heel=np.exp(-t*(42 if surface=='carpet' else 30))*(1-np.exp(-t*700))
        toe=np.exp(-np.maximum(t-.105,0)*40)*(t>.105)*(1-np.exp(-np.maximum(t-.105,0)*500))
        x=noise(len(t),110,2100 if surface=='carpet' else 6500,.3)*(heel+.4*toe)*(.18 if surface=='carpet' else .13)
        if surface!='carpet':
            for freq,gain in [(175+v*8,.16),(421+v*11,.06),(1130+v*17,.025)]:
                x+=np.sin(2*np.pi*freq*t)*np.exp(-t*(18 if surface=='metal' else 55))*gain
        save(f'step_{surface}_{v}',x)

# Four understated physical signatures: felt, ceramic, timber, tensioned metal.
for v in range(4):
    t=np.arange(int(SR*1.2))/SR
    env=(1-np.exp(-t*65))*np.exp(-t*(9+v))
    x=noise(len(t),240,1800+v*800,.25)*env*.024
    for freq,gain in [(210+v*173,.028),(437+v*311,.011),(973+v*503,.004)]:
        x+=np.sin(2*np.pi*freq*t)*env*gain*(0.2 if v==0 else 1)
    save(f'shift_{v}',wet(x,.3))

for cue,frequency in [('anchor',392),('release',261.63),('door',73),('signal',523.25)]:
    t=np.arange(SR*2)/SR
    if cue=='door':
        env=np.sin(np.pi*np.minimum(t/1.6,1))*(t<1.6)
        x=noise(len(t),45,900,.65)*env*.11+np.sin(t*2*np.pi*frequency)*env*.09
    else:
        x=np.zeros(len(t))
        for ratio,amplitude in [(1,.17),(2.01,.05),(3.97,.022),(5.2,.012)]:
            x+=np.sin(t*2*np.pi*frequency*ratio)*np.exp(-t*(4+ratio))*amplitude
        x+=noise(len(t),300,4200,.2)*np.exp(-t*75)*.11
        x*=np.minimum(t*250,1)
    save(cue,wet(x,.9))

# Quiet, episodic room detail. No continuous fan fundamental or broadband rumble.
duration=48
t=np.arange(SR*duration)/SR
for theme in range(3):
    base=noise(len(t),350,2600,.3)*.0018
    for j,at in enumerate([3.2,10.8,19.4,31.7,42.1]):
        age=np.maximum(t-at,0)
        env=(t>=at)*(1-np.exp(-age*9))*np.exp(-age*(1.4 if theme==1 else .7))
        freq=[310,780,164][theme]*(1+j*.127)
        base+=np.sin(2*np.pi*freq*age)*env*.015
        base+=noise(len(t),600,3400,.4)*env*.006
    stereo=wet(base,.65)
    ramp=np.minimum(np.minimum(t/.1,(duration-t)/.1),1)
    save('ambience_'+str(theme),stereo*ramp[:,None])

# Four 24-second suspended chords; no borrowed melody. Circular overlap makes a seamless loop.
duration=96; n=duration*SR; music=np.zeros((n,2)); tt=np.arange(30*SR)/SR
chords=[[146.832,174.614,220,329.628],[116.541,174.614,220,261.626],[130.813,174.614,261.626,293.665],[130.813,195.998,293.665,329.628]]
for bar,notes in enumerate(chords):
    env=np.sin(np.pi*tt/30)**2
    for j,freq in enumerate(notes):
        pad=(np.sin(2*np.pi*freq*tt)+.28*np.sin(2*np.pi*freq*2.002*tt))*.013*env
        indices=(np.arange(len(tt))+bar*24*SR)%n
        music[indices,j%2]+=pad; music[indices,1-j%2]+=pad*.65
    for j,at in enumerate([5,12,19]):
        tau=np.arange(7*SR)/SR; freq=notes[(j+bar)%4]*2
        bell=(np.sin(2*np.pi*freq*tau)+.18*np.sin(2*np.pi*freq*2.003*tau))*np.exp(-tau*1.15)*(1-np.exp(-tau*45))*.045
        indices=(np.arange(len(tau))+(bar*24+at)*SR)%n
        music[indices,j%2]+=bell; music[indices,1-j%2]+=bell*.5
save('score',music)
t=np.arange(48*SR)/SR
outdoors=noise(len(t),90,5000,.75)*.013
for start in [4.2,4.7,11.1,17.3,18.2,29.2,36.8,42.1]:
    age=np.maximum(t-start,0)
    outdoors+=(t>start)*(age<.38)*np.sin(2*np.pi*(2200*age+900*age*age))*np.sin(np.pi*np.minimum(age/.38,1))**2*.035
save('outside',np.column_stack((outdoors,np.roll(outdoors,401))))
print('Generated',len(list(ROOT.glob('*.wav'))),'original audio assets.')
